using Microservice.Models.Matches;

namespace Microservice.Test.Unit;

[TestClass]
public class MatchTests
{
    [TestMethod]
    public void Scores_IsCorrectOrder_TwoTeams()
    {
        // Arrange
        Match match = new("match", new string[] { "team1", "team2" });

        // Act
        match.SetScore("team1", 2);
        match.SetScore("team2", 1);

        // Assert
        Assert.AreEqual("team1", match.Standings[0]);
        Assert.AreEqual("team2", match.Standings[1]);
    }

    [TestMethod]
    public void Scores_IsCorrectOrder_FourTeams()
    {
        // Arrange
        Match match = new(
            "match",
            new string[] { "team1", "team2", "team3", "team4" }
        );

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
    [Ignore]
    public void Scores_IsCorrectOrder_TiedScore()
    {
        // TODO: Implement seeding to handle tied games
    }

    [TestMethod]
    public void Match_CreatesNew()
    {
        // Arrange
        string id = "id";
        string[] teams = new[] { "team1", "team2" };

        // Act
        Match match = new(id, teams);

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
        
        // Act
        Match match = new(id, scores);

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

    // Act
    Match match = new(id, scores, true);

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
        Match match = new("match", new string[] { "team1", "team2" });

        // Act
        match.Start();

        // Assert
        Assert.AreEqual(match.State, MatchState.InProgress);
    }

    [TestMethod]
    public void SetScore_UpdatesScores()
    {
        // Arrange
        Match match = new("match", new string[] { "team1", "team2" });

        // Act
        match.SetScore("team1", 1);
        match.SetScore("team2", 2);

        // Assert
        Assert.AreEqual(match.Scores["team1"], 1);
        Assert.AreEqual(match.Scores["team2"], 2);
    }

    [TestMethod]
    public void Finish_UpdatesState()
    {
        // Arrange
        Match match = new("match", new string[] { "team1", "team2" });

        // Act
        match.Start();
        match.Finish();

        // Assert
        Assert.AreEqual(match.State, MatchState.Completed);
    }
}