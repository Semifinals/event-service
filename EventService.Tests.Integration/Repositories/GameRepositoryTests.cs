using Semifinals.EventService.Models;
using Semifinals.EventService.Utils;
using Semifinals.EventService.Utils.Exceptions;

namespace Semifinals.EventService.Repositories.Tests;

[TestClass]
public class GameRepositoryTests
{
    [TestMethod]
    public async Task GetGameByIdAsync_GetsExistingGame()
    {
        // Arrange
        string expectedId = "";
        string expectedPartitionKey = "";
        Game expectedGame = new()
        {
            Id = expectedId,
            PartitionKey = expectedPartitionKey,
            Index = 0,
            Status = GameStatus.InProgress
            // ... and so on
        };
        
        Mock<ItemResponse<Game>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(expectedGame);

        Mock<Container> container = new();
        container
            .Setup(x => x.ReadItemAsync<Game>(It.IsAny<string>(), It.IsAny<PartitionKey>(), null, default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        Mock<IGraphClient> graphClient = new();

        GameRepository gameRepository = new(graphClient.Object, cosmosClient.Object);

        // Act
        Game game = await gameRepository.GetGameByIdAsync(expectedId, new PartitionKey(expectedPartitionKey));

        // Assert
        Assert.AreEqual(expectedId, game.Id);
    }

    [TestMethod]
    public async Task GetGameByIdAsync_FailsToGetNonExistingGame()
    {
        // Arrange
        string expectedId = "";
        string expectedPartitionKey = "";

        Mock<Container> container = new();
        container
            .Setup(x => x.ReadItemAsync<Game>(It.IsAny<string>(), It.IsAny<PartitionKey>(), null, default))
            .ThrowsAsync(new CosmosException("", HttpStatusCode.NotFound, 1, "", 1));

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        Mock<IGraphClient> graphClient = new();
        
        GameRepository gameRepository = new(graphClient.Object, cosmosClient.Object);

        // Act
        Task getGame() => gameRepository.GetGameByIdAsync(expectedId, new PartitionKey(expectedPartitionKey));

        // Assert
        await Assert.ThrowsExceptionAsync<GameNotFoundException>(getGame);
    }

    [TestMethod]
    public async Task GetGamesBySetIdAsync_GetsGamesFromSet()
    {
        // Arrange
        string expectedId = "";
        string expectedPartitionKey = "";
        
        IEnumerable<Game> games = new List<Game>()
        {
            new()
            {
                Id = expectedId,
                PartitionKey = expectedPartitionKey,
                Index = 0,
                Status = GameStatus.InProgress
                // ... and so on
            }
        };
        
        Mock<FeedResponse<Game>> feedResponse = new();
        feedResponse
            .Setup(x => x.Resource)
            .Returns(games);
        
        Mock<Container> container = new();
        container
            .Setup(x => x.ReadManyItemsAsync<Game>(It.IsAny<IReadOnlyList<(string, PartitionKey)>>(), null, default))
            .ReturnsAsync(feedResponse.Object);
        
        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        IReadOnlyCollection<GameVertex> gameVertices = new List<GameVertex>()
        {
            new()
            {
                Id = expectedId,
                PartitionKey = expectedPartitionKey,
                Index = 0,
                Status = GameStatus.InProgress
                // ... and so on
            }
        };

        ResultSet<GameVertex> resultSet = new(gameVertices, new Dictionary<string, object>());

        Mock<IGraphClient> graphClient = new();
        graphClient
            .Setup(x => x.SubmitAsync<GameVertex>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), default))
            .ReturnsAsync(resultSet);

        GameRepository gameRepository = new(graphClient.Object, cosmosClient.Object);

        // Act
        IEnumerable<Game> gamesFromSet = await gameRepository.GetGamesBySetIdAsync("", new PartitionKey(""));

        // Assert
        Assert.AreEqual(games.Count(), gamesFromSet.Count());
        Assert.AreEqual(games.ElementAt(0).Id, gamesFromSet.ElementAt(0).Id);
    }

    [TestMethod]
    public async Task GetGamesBySetIdAsync_FailsToGetManyGamesFromNonExistingSet()
    {
        // Arrange
        Mock<FeedResponse<Game>> feedResponse = new();
        feedResponse
            .Setup(x => x.Resource)
            .Returns(new List<Game>());

        Mock<Container> container = new();
        container
            .Setup(x => x.ReadManyItemsAsync<Game>(It.IsAny<IReadOnlyList<(string, PartitionKey)>>(), null, default))
            .ReturnsAsync(feedResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        Mock<IGraphClient> graphClient = new();
        graphClient
            .Setup(x => x.SubmitAsync<GameVertex>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), default))
            .ReturnsAsync(new ResultSet<GameVertex>(new List<GameVertex>(), new Dictionary<string, object>()));

        GameRepository gameRepository = new(graphClient.Object, cosmosClient.Object);

        // Act
        IEnumerable<Game> gamesFromSet = await gameRepository.GetGamesBySetIdAsync("", new PartitionKey(""));

        // Assert
        Assert.AreEqual(0, gamesFromSet.Count());
    }
}
