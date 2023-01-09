using System.Collections.Generic;
using System.Linq;

namespace Microservice.Models.Matches;

/// <summary>
/// A match that takes place between multiple teams.
/// </summary>
public class Match
{
  /// <summary>
  /// The match's ID in the database.
  /// </summary>
  public readonly string Id;

  /// <summary>
  /// An array containing the IDs of all teams in the match.
  /// </summary>
  public string[] Teams;

  /// <summary>
  /// A dictionary where the key is the team's ID and the value is their
  /// current score. If the score is -1, it indicates the team has forfeited.
  /// </summary>
  public readonly Dictionary<string, int> Scores;

  /// <summary>
  /// An array containing the IDs of the teams in the match in order of their
  /// current standing in the match.
  /// </summary>
  public string[] Standings => Scores
    .OrderBy(v => v.Value)
    .Select(v => v.Key)
    .ToArray();

  /// <summary>
  /// The current state of the match.
  /// </summary>
  public MatchState State { get; private set; }

  /// <summary>
  /// Create a new match that hasn't started yet.
  /// </summary>
  /// <param name="id">The match ID</param>
  /// <param name="teams">An array of teams that will compete</param>
  public Match(string id, string[] teams)
  {
    Id = id;
    Teams = teams;
    Scores = teams.ToDictionary(teamId => teamId, teamId => 0);
    State = MatchState.NotStarted;
  }

  /// <summary>
  /// Create a match that is currently in progress or finished.
  /// </summary>
  /// <param name="id">The match ID</param>
  /// <param name="scores">The current scores of the match</param>
  /// <param name="finished">Whether or not the match has finished</param>
  public Match(string id, Dictionary<string, int> scores, bool finished = false)
  {
    Id = id;
    Teams = scores.Keys.ToArray();
    Scores = scores;
    State = finished ? MatchState.Completed : MatchState.InProgress;
  }

  /// <summary>
  /// Start the match.
  /// </summary>
  public void Start()
  {
    State = MatchState.InProgress;
  }

  /// <summary>
  /// Set the score of a team.
  /// </summary>
  /// <param name="teamId">The team ID</param>
  /// <param name="score">The team's new score</param>
  public void SetScore(string teamId, int score)
  {
    Scores[teamId] = score;
  }
  
  /// <summary>
  /// Finish the match.
  /// </summary>
  public void Finish()
  {
    State = MatchState.Completed;
  }
}