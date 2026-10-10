namespace CleanArchitecture.Template.Domain.Common;

/// <summary>
/// A lightweight Result type used across the Application layer so handlers can
/// express an expected failure (not found, conflict, business-rule violation)
/// as a value instead of throwing. Unexpected failures (bugs, infrastructure
/// errors) still throw — this is only for outcomes the caller is meant to branch on.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public string? Error { get; }

    protected Result(bool isSuccess, string? error)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("A successful result cannot carry an error.");

        if (!isSuccess && error is null)
            throw new InvalidOperationException("A failed result must carry an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result Failure(string error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, null);

    public static Result<T> Failure<T>(string error) => new(default, false, error);
}

/// <summary>
/// A <see cref="Result"/> that carries a value on success.
/// </summary>
public class Result<T> : Result
{
    private readonly T? _value;

    protected internal Result(T? value, bool isSuccess, string? error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// The success value. Throws if accessed on a failed result — callers must
    /// check <see cref="Result.IsSuccess"/> first, the same discipline an
    /// Either/fold pattern enforces.
    /// </summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value) : onFailure(Error!);

    public static implicit operator Result<T>(T value) => Success(value);
}
