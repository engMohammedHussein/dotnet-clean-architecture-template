using FluentValidation;
using MediatR;
using ValidationException = CleanArchitecture.Template.Application.Common.Exceptions.ValidationException;

namespace CleanArchitecture.Template.Application.Common.Behaviors;

/// <summary>
/// A MediatR pipeline behavior that runs every registered FluentValidation
/// validator for a request before its handler executes. Handlers themselves
/// never validate input — that's deliberate: it keeps "is this request
/// well-formed" (here) separate from "is this allowed given current state"
/// (the handler, e.g. duplicate-SKU checks).
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
