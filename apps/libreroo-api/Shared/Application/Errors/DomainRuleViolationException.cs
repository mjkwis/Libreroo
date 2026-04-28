namespace Libreroo.Api.Shared.Application.Errors;

public sealed class DomainRuleViolationException : Exception
{
    public DomainRuleViolationException(string message) : base(message)
    {
    }
}
