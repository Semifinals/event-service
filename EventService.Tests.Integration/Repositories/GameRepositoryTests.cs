using Semifinals.EventService.Models;
using Semifinals.EventService.Utils;
using Semifinals.EventService.Utils.Exceptions;
using System.Diagnostics.Eventing.Reader;

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

        SetVertex set = new()
        {
            Id = "",
            PartitionKey = expectedPartitionKey,
            Name = "A",
            Status = SetStatus.InProgress
        };
        
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

        GameRepository.SetAndGamesResponse setAndGamesResponse = new()
        {
            Set = set,
            Games = gameVertices
        };

        Mock<IGraphClient> graphClient = new();
        graphClient
            .Setup(x => x.SubmitWithSingleResultAsync<GameRepository.SetAndGamesResponse>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                default))
            .ReturnsAsync(setAndGamesResponse);

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
        Mock<CosmosClient> cosmosClient = new();

        Mock<IGraphClient> graphClient = new();
        graphClient
            .Setup(x => x.SubmitWithSingleResultAsync<GameRepository.SetAndGamesResponse>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                default))
            .ReturnsAsync(new GameRepository.SetAndGamesResponse());

        GameRepository gameRepository = new(graphClient.Object, cosmosClient.Object);

        // Act
        Task getGames() => gameRepository.GetGamesBySetIdAsync("", new PartitionKey(""));

        // Assert
        await Assert.ThrowsExceptionAsync<SetNotFoundException>(getGames);
    }

    [TestMethod]
    public async Task CreateGameAsync_CreatesNewGame()
    {
        // Arrange
        string expectedId = "id";
        string expectedPartitionKey = "partitionKey";
        int expectedIndex = 0;
        GameStatus expectedStatus = GameStatus.InProgress;
        
        Set set = new()
        {
            Id = "",
            PartitionKey = expectedPartitionKey,
            // etc...
        };

        Game game = new()
        {
            Id = expectedId,
            PartitionKey = expectedPartitionKey,
            Index = expectedIndex,
            Status = expectedStatus
        };

        SetVertex setVertex = new()
        {
            Id = "",
            PartitionKey = expectedPartitionKey,
            Status = SetStatus.InProgress,
            Name = ""
        };

        GameVertex gameVertex = new()
        {
            Id = expectedId,
            PartitionKey = expectedPartitionKey,
            Index = expectedIndex,
            Status = expectedStatus
        };

        GameRepository.SetAndGameResponse setAndGameResponse = new()
        {
            Set = setVertex,
            Game = gameVertex
        };

        Mock<ItemResponse<Set>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(set);

        Mock<ItemResponse<Game>> itemResponseGame = new();
        itemResponseGame
            .Setup(x => x.Resource)
            .Returns(game);
        
        Mock<Container> container = new();
        container
            .Setup(x => x.ReadItemAsync<Set>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                null,
                default))
            .ReturnsAsync(itemResponse.Object);
        container
            .Setup(x => x.CreateItemAsync(
                It.IsAny<Game>(),
                It.IsAny<PartitionKey>(),
                null,
                default))
            .ReturnsAsync(itemResponseGame.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        Mock<IGraphClient> graphClient = new();
        graphClient
            .Setup(x => x.SubmitWithSingleResultAsync<GameRepository.SetAndGameResponse>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                default))
            .ReturnsAsync(setAndGameResponse);

        GameRepository gameRepository = new(graphClient.Object, cosmosClient.Object);

        // Act
        Game createdGame = await gameRepository.CreateGameAsync("", new(expectedPartitionKey), expectedIndex);

        // Assert
        Assert.AreEqual(expectedId, createdGame.Id);
        Assert.AreEqual(expectedPartitionKey, createdGame.PartitionKey);
        Assert.AreEqual(expectedIndex, createdGame.Index);
        Assert.AreEqual(expectedStatus, createdGame.Status);
    }

    [TestMethod]
    public async Task CreateGameAsync_FailsOnNonExistentSet()
    {
        // Arrange
        Mock<Container> container = new();
        container
            .Setup(x => x.ReadItemAsync<Set>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                null,
                default))
            .ThrowsAsync(new CosmosException("", HttpStatusCode.NotFound, 0, "", 0));

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        Mock<IGraphClient> graphClient = new();

        GameRepository gameRepository = new(graphClient.Object, cosmosClient.Object);

        // Act
        Task createGame() => gameRepository.CreateGameAsync("", new PartitionKey(""), 0);

        // Assert
        await Assert.ThrowsExceptionAsync<SetNotFoundException>(createGame);
    }

    [TestMethod]
    public async Task UpdateGameAsync_UpdatesExistingGame()
    {
        // Arrange
        string expectedId = "id";
        string expectedPartitionKey = "partitionKey";
        int expectedIndex = 0;
        GameStatus expectedStatus = GameStatus.InProgress;

        Game game = new()
        {
            Id = expectedId,
            PartitionKey = expectedPartitionKey,
            Index = expectedIndex,
            Status = expectedStatus
        };

        GameVertex gameVertex = new()
        {
            Id = expectedId,
            PartitionKey = expectedPartitionKey,
            Index = expectedIndex,
            Status = expectedStatus
        };

        Mock<ItemResponse<Game>> gameResponse = new();
        gameResponse
            .Setup(x => x.Resource)
            .Returns(game);

        Mock<Container> gameContainer = new();
        gameContainer
            .Setup(x => x.PatchItemAsync<Game>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<IReadOnlyList<PatchOperation>>(),
                null,
                default))
            .ReturnsAsync(gameResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(gameContainer.Object);

        Mock<IGraphClient> graphClient = new();
        graphClient
            .Setup(x => x.SubmitWithSingleResultAsync<GameVertex>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                default))
            .ReturnsAsync(gameVertex);

        GameRepository gameRepository = new(graphClient.Object, cosmosClient.Object);

        // Act
        Game updatedGame = await gameRepository.UpdateGameAsync(
            expectedId,
            new(expectedPartitionKey),
            new List<PatchOperation>());

        // Assert
        Assert.AreEqual(expectedId, updatedGame.Id);
        Assert.AreEqual(expectedPartitionKey, updatedGame.PartitionKey);
        Assert.AreEqual(expectedIndex, updatedGame.Index);
        Assert.AreEqual(expectedStatus, updatedGame.Status);
    }

    [TestMethod]
    public async Task UpdateGameAsync_FailsOnNonExistentGame()
    {
        // Arrange
        Mock<Container> container = new();
        container
            .Setup(x => x.PatchItemAsync<Game>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<IReadOnlyList<PatchOperation>>(),
                null,
                default))
            .ThrowsAsync(new CosmosException("", HttpStatusCode.NotFound, 0, "", 0));

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        Mock<IGraphClient> graphClient = new();

        GameRepository gameRepository = new(graphClient.Object, cosmosClient.Object);

        // Act
        Task updateGame() => gameRepository.UpdateGameAsync("", new(""), new List<PatchOperation>());

        // Assert
        await Assert.ThrowsExceptionAsync<GameNotFoundException>(updateGame);
    }
}
