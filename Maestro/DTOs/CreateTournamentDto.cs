namespace Semifinals.Maestro.DTOs;

public class CreateTournamentDto
{
    [JsonProperty("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonProperty("startTime")]
    [JsonPropertyName("startTime")]
    public DateTime? StartTime { get; set; }
}
