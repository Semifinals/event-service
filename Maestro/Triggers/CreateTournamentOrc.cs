using Semifinals.Maestro.DTOs;
using Semifinals.Maestro.Models;
using System.Threading;

namespace Semifinals.Maestro.Triggers;

public class CreateTournamentOrc
{
    [FunctionName(nameof(CreateTournamentOrc))]
    public async Task Trigger(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var tournament = context.GetInput<Tournament>();

        // Create tournament
        await context.CallActivityAsync(
            nameof(CreateTournamentAct),
            tournament);

        // Start timer to begin tournament
        if (tournament.StartTime is not null)
            context.StartNewOrchestration(
                nameof(StartTournamentOrc),
                tournament,
                context.InstanceId);
    }
}
