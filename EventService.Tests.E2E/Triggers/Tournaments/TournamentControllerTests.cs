using Newtonsoft.Json.Linq;
using System.IO;

namespace Semifinals.Services.EventService.Triggers.Tournaments;

[TestClass]
public class TournamentControllerTests : Test
{
    private readonly string TestToken = "dGVzdA==.ODY0MDA=.iNsbhu5s1rdoPT960fY0Bu7sQAaaP2ysD3RJS9DQUmg=";

    [TestInitialize]
    public void TestInitialize()
    {
        // Setup environment variables stored in the main project's local.settings.json
        string json = File.ReadAllText("../../../../EventService/local.settings.json");
        JObject parsed = JObject.Parse(json).Value<JObject>("Values");

        foreach (var item in parsed)
        {
            Environment.SetEnvironmentVariable(item.Key, item.Value.ToString());
        }

        Environment.SetEnvironmentVariable("TokenSecret", "secret");
    }

    [TestMethod]
    public async Task Post_CreatesTournament()
    {
        // Arrange
        TournamentPostDto dto = new()
        {
            Name = "Example Name",
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2)
        };

        HttpRequest req = await CreateRequest(
            dto,
            authorizationHeader: $"Bearer {TestToken}");

        // Act
        IActionResult res = await new TournamentController().Post(req);

        // Assert
        Assert.IsInstanceOfType(res, typeof(CreatedResult));

        Tournament postResData = (Tournament)((CreatedResult)res).Value;
        Assert.AreEqual(dto.Name, postResData.Name);
    }

    [TestMethod]
    public async Task Get_FetchesExistingTournament()
    {
        // Arrange
        TournamentPostDto dto = new()
        {
            Name = "Example Name",
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2)
        };

        HttpRequest postReq = await CreateRequest(
            dto,
            authorizationHeader: $"Bearer {TestToken}");

        IActionResult postRes = await new TournamentController().Post(postReq);
        Tournament postResData = (Tournament)((CreatedResult)postRes).Value;

        HttpRequest req = await CreateRequest();

        // Act
        IActionResult res = await new TournamentController().Get(req, postResData.Id);

        // Assert
        Assert.IsInstanceOfType(postRes, typeof(CreatedResult));
        Assert.IsInstanceOfType(res, typeof(OkObjectResult));

        Tournament resData = (Tournament)((CreatedResult)postRes).Value;
        Assert.AreEqual(postResData.Id, resData.Id);
    }

    [TestMethod]
    public async Task Get_FailsFindingNonExistentTournament()
    {
        // Arrange
        HttpRequest req = await CreateRequest();

        // Act
        IActionResult res = await new TournamentController().Get(req, "DoesntExist");

        // Assert
        Assert.IsInstanceOfType(res, typeof(NotFoundResult));
    }
}