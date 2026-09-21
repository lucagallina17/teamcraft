namespace TeamCraft.Domain.Exceptions;

public class DomainRuleViolationException : Exception
{
    public DomainRuleViolationException(string message) : base(message) { }
}