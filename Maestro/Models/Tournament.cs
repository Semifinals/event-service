namespace Semifinals.Maestro.Models;

public class Tournament
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateTime? StartTime { get; set; }
}
