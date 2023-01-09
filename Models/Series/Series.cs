using Microservice.Models.Matches;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microservice.Models.Series;

/// <summary>
/// A series of matches where the goal is to win a set number of matches.
/// </summary>
public class Series
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
      teamId => Matches.Values.Sum(m => m.Scores[teamId])
    );

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
  public SeriesState State
  {
    get
    {
      if (
        Matches.Count is 0 ||
        (
          Matches.Count is 1 &&
          Matches.Values.ToArray()[0].State == MatchState.NotStarted
        )
      )
        return SeriesState.NotStarted;
      else if (!Scores.Values.Any(s => s >= Goal))
        return SeriesState.InProgress;
      else
        return SeriesState.Completed;
    }
  }

  /// <summary>
  /// Create a new series that hasn't started yet.
  /// </summary>
  /// <param name="id">The series ID</param>
  /// <param name="goal">The number of matches to win in the series</param>
  /// <param name="teams">The teams participating in the series</param>
  public Series(string id, int goal, string[] teams)
  {
    Id = id;
    Goal = goal;
    Teams = teams;
    Matches = new();
  }

  /// <summary>
  /// Create a series that is currently in progress or finished.
  /// </summary>
  /// <param name="id">The series ID</param>
  /// <param name="goal">The number of matches to win in the series</param>
  /// <param name="matches">The matches taking place in the series</param>
  public Series(string id, int goal, Dictionary<string, Match> matches)
  {
    Id = id;
    Goal = goal;
    Teams = matches.Values.SelectMany(match => match.Teams).ToArray();
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
    if (State is not SeriesState.NotStarted)
      throw new InvalidOperationException(
        "Cannot change the series goal after it has started"
      );

    Goal = goal;
  }
}