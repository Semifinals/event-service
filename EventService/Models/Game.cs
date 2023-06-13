using Semifinals.EventService.Utils.Converters;

namespace Semifinals.EventService.Models;

public class GameVertex
{
    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; }

    [JsonPropertyName("index")]
    public int Index { get; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(GameStatusConverter))]
    public GameStatus Status { get; }

    public GameVertex(
        string id,
        string partitionKey,
        int index,
        GameStatus status)
    {
        Id = id;
        PartitionKey = partitionKey;
        Index = index;
        Status = status;
    }
}

public class Game
{
    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; }

    [JsonPropertyName("index")]
    public int Index { get; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(GameStatusConverter))]
    public GameStatus Status { get; }
    
    [JsonPropertyName("scores")]
    public IDictionary<string, double> Scores { get; }

    [JsonPropertyName("gameDefaultScorePolicy")]
    [JsonConverter(typeof(GameDefaultScorePolicyConverter))]
    public GameDefaultScorePolicy GameDefaultScorePolicy { get; }

    public Game(
        string id,
        string partitionKey,
        int index,
        GameStatus status,
        IDictionary<string, double> scores,
        GameDefaultScorePolicy gameDefaultScorePolicy)
    {
        Id = id;
        PartitionKey = partitionKey;
        Index = index;
        Status = status;
        Scores = scores;
        GameDefaultScorePolicy = gameDefaultScorePolicy;
    }

    public static IDictionary<string, double> GetDefaultScores(GameDefaultScorePolicy policy, IEnumerable<SetTeam> teams)
    {
        Dictionary<string, double> scores = new();
        
        switch (policy)
        {
            case GameDefaultScorePolicy.Zero:
            default:
                teams.ToList().ForEach(team => scores.Add(team.Id, 0));
                break;
        }

        return scores;
    }
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

public enum GameDefaultScorePolicy
{
    /// <summary>
    /// Both teams' scores are set to 0
    /// </summary>
    Zero
}
