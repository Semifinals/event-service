using Semifinals.Maestro.Models;

namespace Semifinals.Maestro.Entities;

public interface ITournamentEntity
{
    Guid Id { get; set; }

    string Name { get; set; }

    DateTime? StartTime { get; set; }

    void Create(Tournament tournament);

    Task<Guid> GetId();

    Task<string> GetName();

    Task<DateTime?> GetStartTime();
}

public class TournamentEntity : ITournamentEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateTime? StartTime { get; set; }

    public void Create(Tournament tournament)
    {
        Id = tournament.Id;
        Name = tournament.Name;
        StartTime = tournament.StartTime;
    }

    public Task<Guid> GetId() => Task.FromResult(Id);

    public Task<string> GetName() => Task.FromResult(Name);

    public Task<DateTime?> GetStartTime() => Task.FromResult(StartTime);

    [FunctionName(nameof(TournamentEntity))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx) =>
        ctx.DispatchAsync<TournamentEntity>();
}
