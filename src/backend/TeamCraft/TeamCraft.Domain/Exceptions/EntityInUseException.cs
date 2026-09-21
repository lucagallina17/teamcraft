namespace TeamCraft.Domain.Exceptions;

public class EntityInUseException : Exception
{
    public EntityInUseException(string message) : base(message) { }
}