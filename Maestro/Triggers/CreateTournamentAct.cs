using Semifinals.Maestro.DTOs;
using Semifinals.Maestro.Entities;
using Semifinals.Maestro.Models;

namespace Semifinals.Maestro.Triggers;

public class CreateTournamentAct
{
    [FunctionName(nameof(StartTournamentAct))]
    public async Task Trigger(
        [ActivityTrigger] IDurableActivityContext context,
        [DurableClient] IDurableEntityClient entityClient)
    {
        var tournament = context.GetInput<Tournament>();

        await entityClient.SignalEntityAsync<ITournamentEntity>(
            tournament.Id.ToString(),
            e => e.Create(tournament));
    }
}
