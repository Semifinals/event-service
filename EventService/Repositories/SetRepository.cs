using Semifinals.EventService.Models;
using Semifinals.EventService.Utils.Exceptions;

namespace Semifinals.EventService.Repositories;

public interface ISetRepository
{
    Task<Set> GetSetByIdAsync(
        string id,
        PartitionKey partitionKey);
}

public class SetRepository : ISetRepository
{
    private readonly CosmosClient _cosmosClient;

    public SetRepository(
        CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    public async Task<Set> GetSetByIdAsync(
        string id,
        PartitionKey partitionKey)
    {
        Container container = _cosmosClient.GetContainer("events-db", "matches");

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

    // TODO: Make methods to create/update sets
}