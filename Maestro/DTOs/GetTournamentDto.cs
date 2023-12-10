namespace Semifinals.Maestro.DTOs;

public class GetTournamentDto
{
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonProperty("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonProperty("startTime")]
    [JsonPropertyName("startTime")]
    public DateTime? StartTime { get; set; }
}
