namespace Semifinals.EventService.Utils.Exceptions;

public class GameNotFoundException : NotFoundException
{
    public GameNotFoundException(string id) : base(id) { }
}
