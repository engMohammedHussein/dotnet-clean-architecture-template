namespace CleanArchitecture.Template.Domain.Exceptions;

/// <summary>
/// Base type for exceptions that represent a violation of a domain invariant
/// (as opposed to an infrastructure failure). Thrown by entities themselves
/// when they're asked to enter an invalid state.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}
