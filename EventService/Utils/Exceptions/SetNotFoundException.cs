namespace Semifinals.EventService.Utils.Exceptions;

public class SetNotFoundException : NotFoundException
{
    public SetNotFoundException(string id) : base(id) { }
}
