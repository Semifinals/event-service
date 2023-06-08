using Semifinals.EventService.Utils.Converters;

namespace Semifinals.EventService.Models;

public class SetVertex
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!; // An empty string is used instead of null

    [JsonPropertyName("status")]
    [JsonConverter(typeof(SetStatusConverter))]
    public SetStatus Status { get; set; }
}

public class Set
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; set; } = null!;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(SetStatusConverter))]
    public SetStatus Status { get; set; }

    [JsonPropertyName("teams")]
    public IEnumerable<SetTeam> Teams { get; set; } = null!;

    [JsonPropertyName("scores")]
    public IDictionary<string, double> Scores { get; set; } = null!;

    [JsonPropertyName("scheduledStartAt")]
    [JsonConverter(typeof(UnixEpochConverter))]
    public DateTime? ScheduledStartAt { get; set; }

    [JsonPropertyName("_ts")]
    [JsonConverter(typeof(UnixEpochConverter))]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("prepareStartedAt")]
    [JsonConverter(typeof(UnixEpochConverter))]
    public DateTime? PrepareStartedAt { get; set; }

    [JsonPropertyName("startedAt")]
    [JsonConverter(typeof(UnixEpochConverter))]
    public DateTime? StartedAt { get; set; }

    [JsonPropertyName("confirmStartedAt")]
    [JsonConverter(typeof(UnixEpochConverter))]
    public DateTime? ConfirmStartedAt { get; set; }

    [JsonPropertyName("finishedAt")]
    [JsonConverter(typeof(UnixEpochConverter))]
    public DateTime? FinishedAt { get; set; }
}

public enum SetStatus
{
    /// <summary>
    /// The set has not started yet.
    /// </summary>
    NotStarted,

    /// <summary>
    /// The set is ready to start and waiting on players readying up.
    /// </summary>
    Preparing,
    
    /// <summary>
    /// The set is currently in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// The set is completed and awaiting confirmation of score report.
    /// </summary>
    Confirming,

    /// <summary>
    /// The set is finished.
    /// </summary>
    Finished
}

public class SetTeam
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
