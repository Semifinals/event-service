using Semifinals.Maestro.Models;

namespace Semifinals.Maestro.Triggers;

public class StartTournamentAct
{
    [FunctionName(nameof(StartTournamentAct))]
    public async Task Trigger(
        [ActivityTrigger] IDurableActivityContext context)
    {
        var tournament = context.GetInput<Tournament>();

        await Task.Delay(1);
        throw new NotImplementedException();
    }
}
