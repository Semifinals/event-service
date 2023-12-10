using Semifinals.Maestro.DTOs;
using Semifinals.Maestro.Models;

namespace Semifinals.Maestro.Triggers;

public class StartTournamentOrc
{
    public async Task Trigger(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var tournament = context.GetInput<Tournament>();

        if (tournament.StartTime is null)
            throw new InvalidOperationException(
                "Cannot start timer with no start date set for tournament");

        using var cts = new CancellationTokenSource();
        var timer = context.CreateTimer(
            (DateTime)tournament.StartTime,
            cts.Token);

        var cancel = context.WaitForExternalEvent<int>(
            "CancelScheduledTournamentTimer");

        Task winner = await Task.WhenAny(timer, cancel);
        if (winner == timer)
            await context.CallActivityAsync(
                nameof(StartTournamentAct),
                tournament);

        if (!timer.IsCompleted)
            cts.Cancel();
    }
}
