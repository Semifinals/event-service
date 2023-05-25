using Semifinals.EventService.Models;
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

        SetRepository setRepository = new(cosmosClient.Object);

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

        SetRepository setRepository = new(cosmosClient.Object);

        // Act
        Task getSet() => setRepository.GetSetByIdAsync(expectedId, new(expectedPartitionKey));

        // Assert
        await Assert.ThrowsExceptionAsync<SetNotFoundException>(getSet);
    }
}
