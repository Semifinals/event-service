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
  public Dictionary<string, int> Scores;

  /// <summary>
  /// Flags if the score is simply marking win/loss or if the scores entered
  /// are accurate.
  /// </summary>
  public bool IsDetailedScores;

  /// <summary>
  /// An array in order of the seeding of the match from highest to lowest.
  /// </summary>
  public string[] Seeds;

  /// <summary>
  /// An array containing the IDs of the teams in the match in order of their
  /// seeding and then current standing in the match.
  /// </summary>
  public string[] Standings => Scores
    .OrderBy(score => 
      Seeds
        .Select((v, i) => new { Value = v, Index = i })
        .Where(p => p.Value == score.Key)
        .Select(p => -p.Index)
        .First())
    .OrderBy(v => v.Value)
    .Reverse()
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
  public Match(string id, string[] teams, string[] seeds)
  {
    Id = id;
    Teams = teams;
    Scores = teams.ToDictionary(teamId => teamId, teamId => 0);
    Seeds = seeds;
    State = MatchState.NotStarted;
  }

  /// <summary>
  /// Create a match that is currently in progress or finished.
  /// </summary>
  /// <param name="id">The match ID</param>
  /// <param name="scores">The current scores of the match</param>
  /// <param name="finished">Whether or not the match has finished</param>
  public Match(string id, Dictionary<string, int> scores, string[] seeds, bool detailedScore = true, bool finished = false)
  {
    Id = id;
    Teams = scores.Keys.ToArray();
    Scores = scores;
    Seeds = seeds;
    IsDetailedScores = detailedScore;
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
  /// Flag the match as using a detailed or basic win/loss to record score.
  /// </summary>
  /// <param name="isDetailed">Whether or not the score is detailed</param>
  public void SetDetailedScore(bool isDetailed)
  {
    IsDetailedScores = isDetailed;
  }

  /// <summary>
  /// Finish the match.
  /// </summary>
  public void Finish()
  {
    State = MatchState.Completed;
  }
}