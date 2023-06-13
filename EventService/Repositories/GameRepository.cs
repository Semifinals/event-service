using Semifinals.EventService.Models;
using Semifinals.EventService.Utils;
using Semifinals.EventService.Utils.Exceptions;

namespace Semifinals.EventService.Repositories;

public interface IGameRepository
{
    Task<Game> GetGameByIdAsync(
        string id,
        PartitionKey partitionKey);

    Task<IEnumerable<Game>> GetGamesBySetIdAsync(
        string setId,
        PartitionKey partitionKey);

    Task<Game> CreateGameAsync(
        string setId,
        PartitionKey partitionKey,
        int index,
        GameDefaultScorePolicy defaultScorePolicy,
        IEnumerable<SetTeam> teams);

    Task<Game> UpdateGameAsync(
        string id,
        PartitionKey partitionKey,
        IEnumerable<PatchOperation> operations);
}

public class GameRepository : IGameRepository
{
    private readonly IGraphClient _graphClient;

    private readonly CosmosClient _cosmosClient;
    
    public GameRepository(
        IGraphClient graphClient,
        CosmosClient cosmosClient)
    {
        _graphClient = graphClient;
        _cosmosClient = cosmosClient;
    }
    
    public async Task<Game> GetGameByIdAsync(
        string id,
        PartitionKey partitionKey)
    {
        Container container = _cosmosClient.GetContainer("events-db", "games");

        try
        {
            var res = await container.ReadItemAsync<Game>(id, partitionKey);

            return res.Resource;
        }
        catch (CosmosException)
        {
            throw new GameNotFoundException(id);
        }
    }
    
    public class SetAndGamesResponse
    {
        [JsonPropertyName("set")]
        public SetVertex? Set { get; set; } = null;

        [JsonPropertyName("games")]
        public IEnumerable<GameVertex> Games { get; set; } = Array.Empty<GameVertex>();
    }

    public async Task<IEnumerable<Game>> GetGamesBySetIdAsync(
        string setId,
        PartitionKey partitionKey)
    {
        var results = await _graphClient.SubmitWithSingleResultAsync<SetAndGamesResponse>(
            $@"
                g.V([@setId, @partitionKey])
                    .hasLabel('set')
                    .as('set')
                .outE('hasGame')
                .inV()
                    .fold()
                    .as('games')
                .select('set', 'games')",
            bindings: new Dictionary<string, object>
            {
                { "@setId", setId },
                { "@partitionKey", partitionKey }
            });
        
        if (results.Set == null)
            throw new SetNotFoundException(setId);

        if (!results.Games.Any())
            return Array.Empty<Game>();

        List<(string, PartitionKey)> keys = results.Games
            .Select(result => ((string)result.Id, partitionKey))
            .ToList();

        Container container = _cosmosClient.GetContainer("events-db", "games");
        var res = await container.ReadManyItemsAsync<Game>(keys);

        return res.Resource;
    }

    public class SetAndGameResponse
    {
        public SetVertex? Set { get; set; } = null;

        public GameVertex? Game { get; set; } = null;
    }

    public async Task<Game> CreateGameAsync(
        string setId,
        PartitionKey partitionKey,
        int index,
        GameDefaultScorePolicy defaultScorePolicy,
        IEnumerable<SetTeam> teams)
    {
        // Check that the set exists
        try
        {
            Container setContainer = _cosmosClient.GetContainer("events-db", "sets");
            ItemResponse<Set> set = await setContainer.ReadItemAsync<Set>(setId, partitionKey);
        }
        catch (CosmosException)
        {
            throw new SetNotFoundException(setId);
        }

        // Create models
        string id = ""; // TODO: Generate ID

        Game game = new(
            id,
            partitionKey.ToString(),
            index,
            GameStatus.InProgress,
            Game.GetDefaultScores(defaultScorePolicy, teams),
            defaultScorePolicy);

        GameVertex gameVertex = new(
            id,
            partitionKey.ToString(),
            index,
            GameStatus.InProgress);

        // Update databases
        Container container = _cosmosClient.GetContainer("events-db", "games");
        Task<ItemResponse<Game>> createdGameResponse = container.CreateItemAsync(game, partitionKey);

        Task<GameVertex> createdVertex = _graphClient.SubmitWithSingleResultAsync<GameVertex>(
            $@"
                g.addV('game')
                    .property('id', @id)
                    .property('partiionKey', @partitionKey)
                    .property('index', @index)
                    .property('status', @status)
                    .as('game')
                .sideEffect(
                    __.V([@setId, @partitionKey])
                        .hasLabel('set')
                        .as('set')
                    .addE('hasGame')
                        .from('set')
                        .to('game')
                    .addE('fromSet')
                        .from('game')
                        .to('set'))",
            new Dictionary<string, object>()
            {
                { "@id", gameVertex.Id },
                { "@setId", setId },
                { "@partitionKey", gameVertex.PartitionKey },
                { "@index", gameVertex.Index },
                { "@status", (int)gameVertex.Status }
            });

        await Task.WhenAll(createdGameResponse, createdVertex);

        return createdGameResponse.Result.Resource;
    }

    public async Task<Game> UpdateGameAsync(
        string id,
        PartitionKey partitionKey,
        IEnumerable<PatchOperation> operations)
    {
        // Update document
        Container container = _cosmosClient.GetContainer("events-db", "games");

        Game updatedGame;
        try
        {
            ItemResponse<Game> updatedGameResponse = await container.PatchItemAsync<Game>(
                id,
                partitionKey,
                operations.ToList());
            updatedGame = updatedGameResponse.Resource;
        }
        catch (CosmosException)
        {
            throw new GameNotFoundException(id);
        }

        // Update graph
        await _graphClient.SubmitWithSingleResultAsync<GameVertex>(
            $@"
                g.V([@id, @partitionKey])
                    .hasLabel('set')
                    .property('status', @status)
            ",
            new Dictionary<string, object>()
            {
                { "@id", id },
                { "@partitionKey", partitionKey },
                { "@status", (int)updatedGame.Status },
            });

        // Return with updated set
        return updatedGame;
    }
}
