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

    public async Task<IEnumerable<Game>> GetGamesBySetIdAsync(
        string setId,
        PartitionKey partitionKey)
    {
        var results = await _graphClient.SubmitAsync<GameVertex>(
            "g.V([@setId, @partitionKey]).hasLabel('set').outE('inSet').inV()",
            bindings: new Dictionary<string, object>
            {
                { "@setId", setId },
                { "@partitionKey", partitionKey }
            });

        // TODO: Add check to see if set exists, and throw SetNotFoundException if not

        if (results.Count == 0)
            return Array.Empty<Game>();

        List<(string, PartitionKey)> keys = results
            .Select(result => ((string)result.Id, partitionKey))
            .ToList();

        Container container = _cosmosClient.GetContainer("events-db", "games");
        var res = await container.ReadManyItemsAsync<Game>(keys);

        return res.Resource;
    }

    // TODO: Make methods to create/update games
}