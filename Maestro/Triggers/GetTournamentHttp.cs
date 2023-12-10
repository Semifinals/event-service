using Semifinals.Maestro.DTOs;
using Semifinals.Maestro.Entities;

namespace Semifinals.Maestro.Triggers;

public class GetTournamentHttp
{
    [FunctionName(nameof(GetTournamentHttp))]
    public async Task<IActionResult> Trigger(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "/tournaments/{id}")] HttpRequest req,
        string id,
        [DurableClient] IDurableEntityClient entityClient)
    {
        // Fetch entity
        var entityId = new EntityId(
            nameof(TournamentEntity),
            id);

        var entity = await entityClient.ReadEntityStateAsync<ITournamentEntity>(
            entityId);

        // Return 404 if not exists
        if (!entity.EntityExists)
            return new NotFoundResult();

        // Return dto if exists
        var dto = new GetTournamentDto
        {
            Id = entity.EntityState.Id,
            Name = entity.EntityState.Name,
            StartTime = entity.EntityState.StartTime
        };

        return new OkObjectResult(dto);
    }
}
