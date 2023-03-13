using Microservice.Models.Matches;
using Microservice.Models.Progressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microservice.Models.Series;

/// <summary>
/// A series of matches where the goal is to win a set number of matches.
/// </summary>
public class Set : IProgression
{
  /// <summary>
  /// The series' ID in the database.
  /// </summary>
  public readonly string Id;

  /// <summary>
  /// The number of matches to win to win the series.
  /// </summary>
  public int Goal { get; private set; }

  /// <summary>
  /// An array containing the IDs of all teams in the match.
  /// </summary>
  public string[] Teams;

  // TODO: Add seeding to handle ties
  public string[] Seeds;

  /// <summary>
  /// An array containing the teams in the set that have forfeited.
  /// </summary>
  public string[] Forfeits;

  /// <summary>
  /// A dictionary where the key is the match ID and the value is the match
  /// itself. 
  /// </summary>
  public Dictionary<string, Match> Matches;

  /// <summary>
  /// A dictionary where the key is the team's ID and the value is their
  /// current score. If the score is -1, it indicates the team has forfeited.
  /// </summary>
  public Dictionary<string, int> Scores =>
    Teams.ToDictionary(
      teamId => teamId,
      teamId => Forfeits.Contains(teamId)
        ? -1
        : Matches.Values
          .Where(match => match.Standings[0] == teamId)
          .Count()
    );

  /// <summary>
  /// An array containing the IDs of the teams in the match in order of their
  /// current standing in the match.
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
  public SetState State
  {
    get
    {
      if (
        Scores.Values.Any(s => s >= Goal) ||
        Scores.Values.Where(score => score != -1).Count() <= 1
      )
        return SetState.Completed;
      else if (
        Matches.Count is 0 ||
        (
          Matches.Count is 1 &&
          Matches.Values.ToArray()[0].State == MatchState.NotStarted
        )
      )
        return SetState.NotStarted;
      else
        return SetState.InProgress;
    }
  }

  /// <summary>
  /// Create a new series that hasn't started yet.
  /// </summary>
  /// <param name="id">The series ID</param>
  /// <param name="goal">The number of matches to win in the series</param>
  /// <param name="teams">The teams participating in the series</param>
  public Set(string id, int goal, string[] teams, string[] seeds)
  {
    Id = id;
    Goal = goal;
    Teams = teams;
    Seeds = seeds;
    Forfeits = Array.Empty<string>();
    Matches = new();
  }

  /// <summary>
  /// Create a series that is currently in progress or finished.
  /// </summary>
  /// <param name="id">The series ID</param>
  /// <param name="goal">The number of matches to win in the series</param>
  /// <param name="matches">The matches taking place in the series</param>
  public Set(
    string id,
    int goal,
    Dictionary<string, Match> matches,
    string[] seeds,
    [Optional] string[] forfeits)
  {
    Id = id;
    Goal = goal;
    Teams = matches.Values.SelectMany(match => match.Teams).Distinct().ToArray();
    Seeds = seeds;
    Forfeits = forfeits ?? Array.Empty<string>();
    Matches = matches;
  }

  /// <summary>
  /// Set the target number of match wins a team needs to get to win the
  /// series.
  /// </summary>
  /// <param name="goal">The number of matches to win</param>
  /// <exception cref="InvalidOperationException"></exception>
  public void SetGoal(int goal)
  {
    if (State is not SetState.NotStarted)
      throw new InvalidOperationException(
        "Cannot change the series goal after it has started"
      );

    Goal = goal;
  }
  
  /// <summary>
  /// Mark a team as forfeited.
  /// </summary>
  /// <param name="teamId">The team to forfeit</param>
  public void Forfeit(string teamId)
  {
    if (!Forfeits.Contains(teamId))
      Forfeits = Forfeits.Append(teamId).ToArray();
  }

  #region progression

  /// <summary>
  /// The progression used in composition.
  /// </summary>
  private readonly Progression Progression = new();

  public IProgression[] Progressions => Progression.Progressions;

  #endregion
}