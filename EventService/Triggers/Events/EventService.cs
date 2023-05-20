namespace Semifinals.Services.EventService.Triggers.Events;

public class EventService : IService
{
    private readonly CosmosService Cosmos = new(
        Environment.GetEnvironmentVariable("DbConnectionString")!);

    public async Task<Event> CreateEvent(
        string creatorId,
        string tournamentId,
        string name,
        DateTime stateTime,
        DateTime endTime)
    {
        ///
        /// Current through process: Tournaments exist on "tournament-db"/"tournaments",
        /// so maybe events can be stored as an array on the tournament document itself?
        /// It'd need to remove them from the TournamentService.FindTournament and all
        /// that stuff so those queries aren't massive, so if that isn't possible then
        /// instead maybe it'd have to be a separate container in "tournament-db". Then,
        /// brackets could be separate containers where each document is a match, and
        /// the games are contained in an array on the match. Should these bracket
        /// containers be in a separate database or in the same one? What about the case
        /// of a single event that doesn't necessarily NEED a tournament, is that still
        /// something to make? Maybe they could just use the same ID for the tournament
        /// and event?
        ///
    }

    public async Task<Event?> FindEvent(
        string tournamentId,
        string eventId)
    {
        // Access tournaments container
        Container container = await Cosmos.GetContainerAsync(
            "tournament-db",
            "tournaments",
            "/id");

        // Find tournament
        try
        {
            Tournament tournament = await Cosmos.ReadItemAsync<Tournament>(
                container,
                tournamentId,
                tournamentId);
        }
        catch (Exception)
        {
            return null;
        }
    }
}