using Microservice.Models.Matches;

namespace Microservice.Test.Unit;

[TestClass]
public class MatchTests
{
  [TestMethod]
  public void Standings_IsCorrectOrder_TwoTeams()
  {
    // Arrange
    Match match = new(
      "match",
      new string[] { "team1", "team2" },
      new string[] { "team1", "team2" });

    // Act
    match.SetScore("team1", 2);
    match.SetScore("team2", 1);

    // Assert
    Assert.AreEqual("team1", match.Standings[0]);
    Assert.AreEqual("team2", match.Standings[1]);
  }

  [TestMethod]
  public void Standings_IsCorrectOrder_FourTeams()
  {
    // Arrange
    Match match = new(
        "match",
        new string[] { "team1", "team2", "team3", "team4" },
        new string[] { "team1", "team2", "team3", "team4" });

    // Act
    match.SetScore("team1", 2);
    match.SetScore("team2", 1);
    match.SetScore("team3", 4);
    match.SetScore("team4", 3);

    // Assert
    Assert.AreEqual("team1", match.Standings[2]);
    Assert.AreEqual("team2", match.Standings[3]);
    Assert.AreEqual("team3", match.Standings[0]);
    Assert.AreEqual("team4", match.Standings[1]);
  }

  [TestMethod]
  public void Standings_IsCorrectOrder_TiedScore()
  {
    // Arrange
    Match match = new(
      "match",
      new string[] { "team1", "team2" },
      new string[] { "team1", "team2" });

    // Act
    match.SetScore("team1", 2);
    match.SetScore("team2", 2);

    // Assert
    Assert.AreEqual("team1", match.Standings[0]);
  }

  [TestMethod]
  public void Standings_IsCorrectOrder_TiedScore_AlternateSeeding()
  {
    // Arrange
    Match match = new(
      "match",
      new string[] { "team1", "team2" },
      new string[] { "team2", "team1" });

    // Act
    match.SetScore("team1", 2);
    match.SetScore("team2", 2);

    // Assert
    Assert.AreEqual("team2", match.Standings[0]);
  }

  [TestMethod]
  public void Match_CreatesNew()
  {
    // Arrange
    string id = "id";
    string[] teams = new[] { "team1", "team2" };
    string[] seeds = new[] { "team1", "team2" };

    // Act
    Match match = new(id, teams, seeds);

    // Assert
    Assert.AreEqual(match.Id, id);
    Assert.AreEqual(match.Teams, teams);
    Assert.AreEqual(match.State, MatchState.NotStarted);
  }

  [TestMethod]
  public void Match_CreatesExisting()
  {
    // Arrange
    string id = "id";
    Dictionary<string, int> scores = new()
        {
            { "team1", 1 },
            { "team2", 2 },
        };
    string[] seeds = new[] { "team1", "team2" };

    // Act
    Match match = new(id, scores, seeds);

    // Assert
    Assert.AreEqual(match.Id, id);
    Assert.AreEqual(match.Scores["team1"], 1);
    Assert.AreEqual(match.Scores["team2"], 2);
    Assert.AreEqual(match.State, MatchState.InProgress);
  }

  [TestMethod]
  public void Match_CreatesFinished()
  {
    // Arrange
    string id = "id";
    Dictionary<string, int> scores = new()
        {
            { "team1", 1 },
            { "team2", 2 },
        };
    string[] seeds = new[] { "team1", "team2" };

    // Act
    Match match = new(id, scores, seeds, true, true);

    // Assert
    Assert.AreEqual(match.Id, id);
    Assert.AreEqual(match.Scores["team1"], 1);
    Assert.AreEqual(match.Scores["team2"], 2);
    Assert.AreEqual(match.State, MatchState.Completed);
  }

  [TestMethod]
  public void Start_UpdatesState()
  {
    // Arrange
    Match match = new(
      "match",
      new string[] { "team1", "team2" },
      new string[] { "team1", "team2" });

    // Act
    match.Start();

    // Assert
    Assert.AreEqual(match.State, MatchState.InProgress);
  }

  [TestMethod]
  public void SetScore_UpdatesScores()
  {
    // Arrange
    Match match = new(
      "match",
      new string[] { "team1", "team2" },
      new string[] { "team1", "team2" });

    // Act
    match.SetScore("team1", 1);
    match.SetScore("team2", 2);

    // Assert
    Assert.AreEqual(match.Scores["team1"], 1);
    Assert.AreEqual(match.Scores["team2"], 2);
  }

  [TestMethod]
  public void SetDetailedScore_UpdatesStatus()
  {
    // Arrange
    Match match = new(
      "match",
      new string[] { "team1", "team2" },
      new string[] { "team1", "team2" });

    // Act
    match.SetDetailedScore(false);

    // Assert
    Assert.AreEqual(false, match.IsDetailedScores);
  }

  [TestMethod]
  public void Finish_UpdatesState()
  {
    // Arrange
    Match match = new(
      "match",
      new string[] { "team1", "team2" },
      new string[] { "team1", "team2" });

    // Act
    match.Start();
    match.Finish();

    // Assert
    Assert.AreEqual(match.State, MatchState.Completed);
  }
}