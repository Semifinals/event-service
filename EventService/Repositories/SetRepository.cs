using Semifinals.EventService.Models;
using Semifinals.EventService.Utils;
using Semifinals.EventService.Utils.Exceptions;

namespace Semifinals.EventService.Repositories;

public interface ISetRepository
{
    Task<Set> GetSetByIdAsync(
        string id,
        PartitionKey partitionKey);

    Task<Set> CreateSetAsync(
        PartitionKey partitionKey,
        IEnumerable<SetTeam> teams,
        GameDefaultScorePolicy gameDefaultScorePolicy,
        string? name = null,
        DateTime? scheduledStartAt = null);

    Task<Set> UpdateSetByIdAsync(
        string id,
        PartitionKey partitionKey,
        IEnumerable<PatchOperation> operations);
}

public class SetRepository : ISetRepository
{
    private readonly IGraphClient _graphClient;
    
    private readonly CosmosClient _cosmosClient;

    public SetRepository(
        IGraphClient graphClient,
        CosmosClient cosmosClient)
    {
        _graphClient = graphClient;
        _cosmosClient = cosmosClient;
    }

    public async Task<Set> GetSetByIdAsync(
        string id,
        PartitionKey partitionKey)
    {
        Container container = _cosmosClient.GetContainer("events-db", "sets");

        try
        {
            var res = await container.ReadItemAsync<Set>(id, partitionKey);

            return res.Resource;
        }
        catch (CosmosException)
        {
            throw new SetNotFoundException(id);
        }
    }

    public async Task<Set> CreateSetAsync(
        PartitionKey partitionKey,
        IEnumerable<SetTeam> teams,
        GameDefaultScorePolicy gameDefaultScorePolicy,
        string? name = null,
        DateTime? scheduledStartAt = null)

    {
        // Create models
        string id = ""; // TODO: Generate ID

        IDictionary<string, double> scores = new Dictionary<string, double>();
        teams.ToList().ForEach(team => scores.Add(team.Id, 0));

        Set set = new(
            id,
            partitionKey.ToString(),
            SetStatus.NotStarted,
            teams,
            scores,
            gameDefaultScorePolicy,
            scheduledStartAt: scheduledStartAt,
            name: name);

        SetVertex setVertex = new(
            id,
            partitionKey.ToString(),
            SetStatus.NotStarted,
            name);

        // Update databases
        Container container = _cosmosClient.GetContainer("events-db", "sets");
        ItemResponse<Set> createdSetResponse = await container.CreateItemAsync(set, partitionKey);
        Set createdSet = createdSetResponse.Resource;

        await _graphClient.SubmitWithSingleResultAsync<SetVertex>(
            $@"
                g.addV('set')
                    .property('id', @id)
                    .property('partiionKey', @partitionKey)
                    .property('name', @name)
                    .property('status', @status)",
            new Dictionary<string, object>()
            {
                { "@id", setVertex.Id },
                { "@partitionKey", setVertex.PartitionKey },
                { "@name", setVertex.Name ?? "" },
                { "@status", (int)setVertex.Status }
            });

        return createdSet;
    }
    
    public async Task<Set> UpdateSetByIdAsync(
        string id,
        PartitionKey partitionKey,
        IEnumerable<PatchOperation> operations)
    {
        // Update document
        Container container = _cosmosClient.GetContainer("events-db", "sets");
        Set updatedSet;
        try
        {
            ItemResponse<Set> updatedSetResponse = await container.PatchItemAsync<Set>(
                id,
                partitionKey,
                operations.ToList());
            updatedSet = updatedSetResponse.Resource;
        }
        catch (CosmosException)
        {
            throw new SetNotFoundException(id);
        }

        // Update graph
        await _graphClient.SubmitWithSingleResultAsync<SetVertex>(
            $@"
                g.V([@id, @partitionKey])
                    .hasLabel('set')
                    .property('name', @name)
                    .property('status', @status)
            ",
            new Dictionary<string, object>()
            {
                { "@id", id },
                { "@partitionKey", partitionKey },
                { "@name", updatedSet.Name ?? "" },
                { "@status", (int)updatedSet.Status }
            });

        // Return with updated set
        return updatedSet;
    }
}
