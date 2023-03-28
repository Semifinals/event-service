namespace Semifinals.Services.Event.Triggers.Tournaments;

public class TournamentService : IService
{
    private readonly CosmosService Cosmos = new(
        Environment.GetEnvironmentVariable("DbConnectionString")!);

    /// <summary>
    /// Create a new tournament.
    /// </summary>
    /// <param name="creatorId">The ID of the creator of the tournament</param>
    /// <param name="name">The name of the tournament</param>
    /// <param name="startTime">The start time of the tournament</param>
    /// <param name="endTime">The end time of the tournament</param>
    /// <returns>The created tournament</returns>
    public async Task<Tournament> CreateTournament(
        string creatorId,
        string name,
        DateTime startTime,
        DateTime endTime)
    {
        // Access tournaments container
        Container container = await Cosmos.GetContainerAsync(
            "tournament-db",
            "tournaments",
            "/id");

        // Upload tournament to database
        string id = ""; // TODO: Generate unique ID
        Tournament tournament = new(id, name, startTime, endTime, creatorId);
        Tournament res = await Cosmos.CreateItemAsync(container, tournament, tournament.Id);

        // Respond with created tournament
        return res;
    }

    /// <summary>
    /// Find a tournament by its ID.
    /// </summary>
    /// <param name="id">The ID of the tournament to find</param>
    /// <returns>The tournament if it exists</returns>
    public async Task<Tournament?> FindTournament(string id)
    {
        // Access tournaments container
        Container container = await Cosmos.GetContainerAsync(
            "tournament-db",
            "tournaments",
            "/id");

        // Find tournament in database
        try
        {
            return await Cosmos.ReadItemAsync<Tournament>(container, id, id);
        }
        catch (Exception)
        {
            return null;
        }
    }
}