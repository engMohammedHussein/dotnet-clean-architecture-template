namespace CleanArchitecture.Template.Domain.Exceptions;

/// <summary>
/// Thrown when a lookup by identity fails for an entity that was expected to
/// exist. Mapped to a 404 at the API boundary.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"\"{entityName}\" with identifier ({key}) was not found.")
    {
    }
}
