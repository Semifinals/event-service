using Semifinals.EventService.Utils.Converters;

namespace Semifinals.EventService.Models;

public class GameVertex
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; set; } = null!;

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(GameStatusConverter))]
    public GameStatus Status { get; set; }
}

public class Game
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; set; } = null!;

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(GameStatusConverter))]
    public GameStatus Status { get; set; }

    [JsonPropertyName("scores")]
    public IDictionary<string, int> Scores { get; set; } = null!;
}

public enum GameStatus
{
    /// <summary>
    /// The game is currently in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// The game is completed and awaiting confirmation of score report.
    /// </summary>
    Confirming,

    /// <summary>
    /// The game is finished.
    /// </summary>
    Finished
}
