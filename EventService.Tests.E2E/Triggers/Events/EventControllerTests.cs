using Newtonsoft.Json.Linq;
using System.IO;

namespace Semifinals.Services.EventService.Triggers.Tournaments;

[TestClass]
public class EventControllerTests : Test
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
    public void Post_CreatesEvent()
    {
    }

    [TestMethod]
    public void Post_FailsCreatingWithoutTournament()
    {
    }

    [TestMethod]
    public void Get_FetchesExistingEvent()
    {
    }

    [TestMethod]
    public void Get_FailsFindingNonExistentEvent()
    {
    }
}