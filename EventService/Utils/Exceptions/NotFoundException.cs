namespace Semifinals.EventService.Utils.Exceptions;

public class NotFoundException : Exception
{
    public readonly string Id;
    
    public NotFoundException(string id) : base()
    {
        Id = id;
    }
}
