namespace Semifinals.Services.Event.Models;

public class Tournament : CosmosItem
{
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

    public Tournament(
        string id,
        string name,
        DateTime startTime,
        DateTime endTime,
        string creatorId,
        DateTime? timestamp = null)
    : base(id, timestamp)
    {
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        CreatorId = creatorId;
    }
}