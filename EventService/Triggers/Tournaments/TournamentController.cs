namespace Semifinals.Services.Event.Triggers.Tournaments;

public class TournamentController : Controller<TournamentService>
{
    /// <summary>
    /// Create a new tournament.
    /// </summary>
    /// <param name="req">The HTTP request</param>
    /// <returns>The resulting tournament that was created</returns>
    /// <response code="201">Indicates the tournament was successfully created</response>
    /// <response code="400">Indicates the body of the request was invalid</response>
    /// <response code="401">Indicates the request is missing authorization</response>
    [FunctionName("TournamentPost")]
    public async Task<IActionResult> Post(
        [HttpTrigger(authLevel: AuthorizationLevel.Anonymous, "post", Route = "tournaments")] HttpRequest req)
    {
        return await Function<TournamentPostDto>.Run(
            req,
            true,
            jwtSecret: Environment.GetEnvironmentVariable("JsonWebTokenSecret")!)(async func =>
        {
            // Get user ID from auth
            string creatorId = func.User!.Subject;

            // Create tournament
            Tournament tournament = await Service.CreateTournament(
                creatorId,
                func.Body.Name,
                func.Body.StartTime,
                func.Body.EndTime);

            // Respond with created tournament
            return new CreatedResult(
                $"https://api.semifinals.co/tournaments/{tournament.Id}",
                tournament);
        });
    }

    /// <summary>
    /// Get a tournament by its ID.
    /// </summary>
    /// <param name="req">The HTTP request</param>
    /// <param name="id">The ID of the tournament to fetch</param>
    /// <returns>The tournament with the corresponding ID</returns>
    /// <response code="200">Indicates the tournament was successfully fetched</response>
    /// <response code="404">Indicates the tournament doesn't exist</response>
    [FunctionName("TournamentGet")]
    public async Task<IActionResult> Get(
        [HttpTrigger(authLevel: AuthorizationLevel.Anonymous, "get", Route = "tournaments/{id}")] HttpRequest req,
        string id)
    {
        return await Function.Run(req)(async func =>
        {
            // Fetch tournament
            Tournament? tournament = await Service.FindTournament(id);

            if (tournament is null)
                return new NotFoundResult();

            // Respond with fetched tournament
            return new OkObjectResult(tournament);
        });
    }
}