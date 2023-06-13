using Semifinals.EventService.Utils.Converters;

namespace Semifinals.EventService.Models;

public class SetVertex
{
    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; }

    [JsonPropertyName("name")]
    public string? Name { get; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(SetStatusConverter))]
    public SetStatus Status { get; }

    public SetVertex(
        string id,
        string partitionKey,
        SetStatus status,
        string? name = null)
    {
        Id = id;
        PartitionKey = partitionKey;
        Name = name;
        Status = status;
    }
}

public class Set
{
    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; }

    [JsonPropertyName("name")]
    public string? Name { get; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(SetStatusConverter))]
    public SetStatus Status { get; }

    [JsonPropertyName("teams")]
    public IEnumerable<SetTeam> Teams { get; }

    [JsonPropertyName("scores")]
    public IDictionary<string, double> Scores { get; }

    [JsonPropertyName("gameDefaultScorePolicy")]
    [JsonConverter(typeof(GameDefaultScorePolicyConverter))]
    public GameDefaultScorePolicy GameDefaultScorePolicy { get; }

    [JsonPropertyName("scheduledStartAt")]
    [JsonConverter(typeof(UnixEpochConverter))]
    public DateTime? ScheduledStartAt { get; }

    [JsonPropertyName("_ts")]
    [JsonConverter(typeof(UnixEpochConverter))]
    public DateTime CreatedAt { get; }

    [JsonPropertyName("prepareStartedAt")]
    [JsonConverter(typeof(UnixEpochConverter))]
    public DateTime? PrepareStartedAt { get; }

    [JsonPropertyName("startedAt")]
    [JsonConverter(typeof(UnixEpochConverter))]
    public DateTime? StartedAt { get; }

    [JsonPropertyName("confirmStartedAt")]
    [JsonConverter(typeof(UnixEpochConverter))]
    public DateTime? ConfirmStartedAt { get; }

    [JsonPropertyName("finishedAt")]
    [JsonConverter(typeof(UnixEpochConverter))]
    public DateTime? FinishedAt { get; }

    public Set(
        string id,
        string partitionKey,
        SetStatus status,
        IEnumerable<SetTeam> teams,
        IDictionary<string, double> scores,
        GameDefaultScorePolicy gameDefaultScorePolicy,
        DateTime? createdAt = null,
        DateTime? scheduledStartAt = null,
        DateTime? prepareStartedAt = null,
        DateTime? startedAt = null,
        DateTime? confirmStartedAt = null,
        DateTime? finishedAt = null,
        string? name = null)
    {
        Id = id;
        PartitionKey = partitionKey;
        Name = name;
        Status = status;
        Teams = teams;
        Scores = scores;
        GameDefaultScorePolicy = gameDefaultScorePolicy;
        ScheduledStartAt = scheduledStartAt;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        PrepareStartedAt = prepareStartedAt;
        StartedAt = startedAt;
        ConfirmStartedAt = confirmStartedAt;
        FinishedAt = finishedAt;
    }
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
    public string Id { get; }

    [JsonPropertyName("name")]
    public string Name { get; }
    
    public SetTeam(
        string id,
        string name)
    {
        Id = id;
        Name = name;
    }
}
