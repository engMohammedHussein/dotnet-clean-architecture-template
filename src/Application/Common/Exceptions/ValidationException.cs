using FluentValidation.Results;

namespace CleanArchitecture.Template.Application.Common.Exceptions;

/// <summary>
/// Thrown by <see cref="Behaviors.ValidationBehavior{TRequest,TResponse}"/> when
/// FluentValidation rejects a command or query, with every failure collected
/// by property name rather than just the first one. Mapped to a 400 with a
/// field-level error payload at the API boundary.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}
