using Microsoft.Azure.Cosmos;
using Semifinals.EventService.Models;
using Semifinals.EventService.Utils;
using Semifinals.EventService.Utils.Exceptions;

namespace Semifinals.EventService.Repositories.Tests;

[TestClass]
public class SetRepositoryTests
{
    [TestMethod]
    public async Task GetSetByIdAsync_GetsExistingSet()
    {
        // Arrange
        string expectedId = "";
        string expectedPartitionKey = "";
        Set expectedSet = new()
        {
            Id = expectedId,
            PartitionKey = expectedPartitionKey,
            Name = "A",
            Status = SetStatus.NotStarted
            // ... and so on
        };

        Mock<ItemResponse<Set>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(expectedSet);

        Mock<Container> container = new();
        container
            .Setup(x => x.ReadItemAsync<Set>(It.IsAny<string>(), It.IsAny<PartitionKey>(), null, default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        Mock<IGraphClient> graphClient = new();

        SetRepository setRepository = new(graphClient.Object, cosmosClient.Object);

        // Act
        Set set = await setRepository.GetSetByIdAsync(expectedId, new(expectedPartitionKey));

        // Assert
        Assert.AreEqual(expectedId, set.Id);
    }

    [TestMethod]
    public async Task GetSetByIdAsync_FailsToGetNonExistingSet()
    {
        // Arrange
        string expectedId = "";
        string expectedPartitionKey = "";

        Mock<Container> container = new();
        container
            .Setup(x => x.ReadItemAsync<Set>(It.IsAny<string>(), It.IsAny<PartitionKey>(), null, default))
            .ThrowsAsync(new CosmosException("", HttpStatusCode.NotFound, 1, "", 1));

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        Mock<IGraphClient> graphClient = new();

        SetRepository setRepository = new(graphClient.Object, cosmosClient.Object);

        // Act
        Task getSet() => setRepository.GetSetByIdAsync(expectedId, new(expectedPartitionKey));

        // Assert
        await Assert.ThrowsExceptionAsync<SetNotFoundException>(getSet);
    }

    [TestMethod]
    public async Task CreateSetAsync_CreatesNewSetWithOptionalValues()
    {
        // Arrange
        string expectedPartitionKey = "";
        string expectedName = "A";
        DateTime expectedStartTime = DateTime.UtcNow;

        IEnumerable<SetTeam> teams = new List<SetTeam>()
        {
            new SetTeam
            {
                Id = "team1",
                Name = "Team 1"
            },
            new SetTeam
            {
                Id = "team2",
                Name = "Team 2"
            }
        };

        Set expectedSet = new()
        {
            Id = "id",
            PartitionKey = expectedPartitionKey,
            Name = expectedName,
            Status = SetStatus.NotStarted,
            Teams = teams,
            ScheduledStartAt = expectedStartTime
            // ... and so on
        };

        SetVertex expectedSetVertex = new()
        {
            Id = "id",
            PartitionKey = expectedPartitionKey,
            Name = expectedName,
            Status = SetStatus.NotStarted
        };

        Mock<ItemResponse<Set>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(expectedSet);

        Mock<Container> container = new();
        container
            .Setup(x => x.CreateItemAsync(It.IsAny<Set>(), It.IsAny<PartitionKey>(), null, default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);
        
        Mock<IGraphClient> graphClient = new();
        graphClient
            .Setup(x => x.SubmitWithSingleResultAsync<SetVertex>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                default))
            .ReturnsAsync(expectedSetVertex);
        
        SetRepository setRepository = new(graphClient.Object, cosmosClient.Object);

        // Act
        Set set = await setRepository.CreateSetAsync(new(expectedPartitionKey), teams, expectedName, expectedStartTime);

        // Assert
        Assert.AreEqual(expectedPartitionKey, set.PartitionKey);
        Assert.AreEqual(2, set.Teams.Count());
        Assert.AreEqual(expectedName, set.Name);
        Assert.AreEqual(expectedStartTime, set.ScheduledStartAt);
    }

    [TestMethod]
    public async Task CreateSetAsync_CreatesNewSetWithoutOptionalValues()
    {
        // Arrange
        string expectedPartitionKey = "";

        IEnumerable<SetTeam> teams = new List<SetTeam>()
        {
            new SetTeam
            {
                Id = "team1",
                Name = "Team 1"
            },
            new SetTeam
            {
                Id = "team2",
                Name = "Team 2"
            }
        };

        Set expectedSet = new()
        {
            Id = "id",
            PartitionKey = expectedPartitionKey,
            Status = SetStatus.NotStarted,
            Teams = teams
            // ... and so on
        };

        SetVertex expectedSetVertex = new()
        {
            Id = "id",
            PartitionKey = expectedPartitionKey,
            Status = SetStatus.NotStarted
        };

        Mock<ItemResponse<Set>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(expectedSet);

        Mock<Container> container = new();
        container
            .Setup(x => x.CreateItemAsync(It.IsAny<Set>(), It.IsAny<PartitionKey>(), null, default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        Mock<IGraphClient> graphClient = new();
        graphClient
            .Setup(x => x.SubmitWithSingleResultAsync<SetVertex>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                default))
            .ReturnsAsync(expectedSetVertex);

        SetRepository setRepository = new(graphClient.Object, cosmosClient.Object);

        // Act
        Set set = await setRepository.CreateSetAsync(new(expectedPartitionKey), teams);

        // Assert
        Assert.AreEqual(expectedPartitionKey, set.PartitionKey);
        Assert.AreEqual(2, set.Teams.Count());
        Assert.IsNull(set.Name);
        Assert.IsNull(set.ScheduledStartAt);
    }

    [TestMethod]
    public async Task UpdateSetByIdAsync_AddsMissingOptionalValues()
    {
        // Arrange
        string expectedPartitionKey = "";
        string expectedName = "A";
        DateTime expectedStartTime = DateTime.UtcNow;

        IEnumerable<SetTeam> teams = new List<SetTeam>()
        {
            new SetTeam
            {
                Id = "team1",
                Name = "Team 1"
            },
            new SetTeam
            {
                Id = "team2",
                Name = "Team 2"
            }
        };

        Set expectedSet = new()
        {
            Id = "id",
            PartitionKey = expectedPartitionKey,
            Name = expectedName,
            Status = SetStatus.NotStarted,
            Teams = teams,
            ScheduledStartAt = expectedStartTime
            // ... and so on
        };

        SetVertex expectedSetVertex = new()
        {
            Id = "id",
            PartitionKey = expectedPartitionKey,
            Name = expectedName,
            Status = SetStatus.NotStarted
        };

        Mock<ItemResponse<Set>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(expectedSet);

        Mock<Container> container = new();
        container
            .Setup(x => x.PatchItemAsync<Set>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<IReadOnlyList<PatchOperation>>(),
                null,
                default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        Mock<IGraphClient> graphClient = new();
        graphClient
            .Setup(x => x.SubmitWithSingleResultAsync<SetVertex>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                default))
            .ReturnsAsync(expectedSetVertex);

        SetRepository setRepository = new(graphClient.Object, cosmosClient.Object);

        IEnumerable<PatchOperation> operations = new List<PatchOperation>()
        {
            PatchOperation.Add("/id", "id"),
            PatchOperation.Add("/scheduledStartAt", ((DateTimeOffset)expectedStartTime).ToUnixTimeSeconds())
        };

        // Act
        Set set = await setRepository.UpdateSetByIdAsync("id", new(expectedPartitionKey), operations);

        // Assert
        Assert.AreEqual(expectedPartitionKey, set.PartitionKey);
        Assert.AreEqual(2, set.Teams.Count());
        Assert.AreEqual(expectedName, set.Name);
        Assert.AreEqual(expectedStartTime, set.ScheduledStartAt);
    }

    [TestMethod]
    public async Task UpdateSetByIdAsync_AddsRemovesOptionalValues()
    {
        // Arrange
        string expectedPartitionKey = "";

        IEnumerable<SetTeam> teams = new List<SetTeam>()
        {
            new SetTeam
            {
                Id = "team1",
                Name = "Team 1"
            },
            new SetTeam
            {
                Id = "team2",
                Name = "Team 2"
            }
        };

        Set expectedSet = new()
        {
            Id = "id",
            PartitionKey = expectedPartitionKey,
            Status = SetStatus.NotStarted,
            Teams = teams
            // ... and so on
        };

        SetVertex expectedSetVertex = new()
        {
            Id = "id",
            PartitionKey = expectedPartitionKey,
            Status = SetStatus.NotStarted
        };

        Mock<ItemResponse<Set>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(expectedSet);

        Mock<Container> container = new();
        container
            .Setup(x => x.PatchItemAsync<Set>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<IReadOnlyList<PatchOperation>>(),
                null,
                default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        Mock<IGraphClient> graphClient = new();
        graphClient
            .Setup(x => x.SubmitWithSingleResultAsync<SetVertex>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                default))
            .ReturnsAsync(expectedSetVertex);

        SetRepository setRepository = new(graphClient.Object, cosmosClient.Object);

        IEnumerable<PatchOperation> operations = new List<PatchOperation>()
        {
            PatchOperation.Remove("/id"),
            PatchOperation.Remove("/scheduledStartAt")
        };

        // Act
        Set set = await setRepository.UpdateSetByIdAsync("id", new(expectedPartitionKey), operations);

        // Assert
        Assert.AreEqual(expectedPartitionKey, set.PartitionKey);
        Assert.AreEqual(2, set.Teams.Count());
        Assert.IsNull(set.Name);
        Assert.IsNull(set.ScheduledStartAt);
    }
}
