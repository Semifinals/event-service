namespace Semifinals.Services.EventService.Triggers.Events;

public class EventController : Controller<EventService>
{
    /// <summary>
    /// Create a new event in a tournament.
    /// </summary>
    /// <param name="req">The HTTP request</param>
    /// <param name="tournamentId">The ID of the tournament the event is for</param>
    /// <returns>The resulting event that was created</returns>
    /// <response code="201">Indicates the event was successfully created</response>
    /// <response code="400">Indicates the body of the request was invalid</response>
    /// <response code="401">Indicates the request is missing authorization</response>
    [FunctionName("EventPost")]
    public async Task<IActionResult> Post(
        [HttpTrigger(authLevel: AuthorizationLevel.Anonymous, "post", Route = "tournaments/{tournamentId}/events")] HttpRequest req,
        string tournamentId)
    {
        return await FunctionBuilder.Init()
            .AddBody<EventPostDto>()
            .Build(req, true)(async func =>
            {
                // Get user ID from auth
                string creatorId = Token.GetId(func.User!)!;

                // Create event
                Event ev = await Service.CreateEvent(
                    creatorId,
                    tournamentId,
                    func.Body.Name,
                    func.Body.StartTime,
                    func.Body.EndTime);

                // Respond with created event
                return new CreatedResult(
                    $"https://api.semifinals.co/tournaments/{ev.TournamentId}/events/{ev.Id}",
                    ev);
            });
    }

    /// <summary>
    /// Get an event by its ID.
    /// </summary>
    /// <param name="req">The HTTP request</param>
    /// <param name="tournamentId">The ID of the tournament to fetch from</param>
    /// <param name="eventId">The ID of the event to fetch</param>
    /// <returns>The event with the corresponding ID</returns>
    /// <response code="200">Indicates the event was successfully fetched</response>
    /// <response code="404">Indicates the tournament or event doesn't exist</response>
    [FunctionName("EventGet")]
    public async Task<IActionResult> Get(
        [HttpTrigger(authLevel: AuthorizationLevel.Anonymous, "get", Route = "tournaments/{tournamentId}/events/{eventId}")] HttpRequest req,
        string tournamentId,
        string eventId)
    {
        return await FunctionBuilder.Init().Build(req)(async func =>
            {
                // Fetch event
                Event? ev = await Service.FindEvent(tournamentId, eventId);

                if (ev is null)
                    return new NotFoundResult();

                // Respond with fetched event
                return new OkObjectResult(ev);
            });
    }
}