namespace Semifinals.Services.EventService.Models;

public class Event : CosmosItem
{
    [JsonProperty("tournamentId")]
    public string TournamentId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("startTime")]
    [JsonConverter(typeof(EpochConverter))]
    public DateTime StartTime { get; set; }

    [JsonProperty("endTime")]
    [JsonConverter(typeof(EpochConverter))]
    public DateTime EndTime { get; set; }

    [JsonProperty("creatorId")]
    public string CreatorId { get; set; }

    public Event(
        string id,
        string tournamentId,
        string name,
        DateTime startTime,
        DateTime endTime,
        string creatorId,
        DateTime? timestamp = null)
    : base(id, timestamp)
    {
        TournamentId = tournamentId;
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        CreatorId = creatorId;
    }
}