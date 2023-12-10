using Semifinals.Maestro.DTOs;
using Semifinals.Maestro.Models;

namespace Semifinals.Maestro.Triggers;

public class CreateTournamentHttp
{
    [FunctionName(nameof(CreateTournamentHttp))]
    public async Task<IActionResult> Trigger(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "tournaments")] HttpRequest req,
        [FromBody] CreateTournamentDto dto,
        [DurableClient] IDurableOrchestrationClient orchestrationClient)
    {
        var tournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            StartTime = dto.StartTime
        };

        var res = await orchestrationClient.StartNewAsync(
            nameof(CreateTournamentOrc),
            tournament.Id.ToString(),
            tournament);

        req.HttpContext.Response.Headers.Add("Retry-After", "1");
        return new AcceptedResult(
            $"/tournaments/{tournament.Id}",
            "Tournament scheduled for processing");
    }
}
