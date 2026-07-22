namespace SportsGurukul.Application.Common.Models;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public IReadOnlyList<string> Errors { get; }

    private Result(bool isSuccess, T? value, string? error, IReadOnlyList<string> errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Errors = errors;
    }

    public static Result<T> Success(T value) =>
        new(true, value, null, Array.Empty<string>());

    public static Result<T> Failure(string error) =>
        new(false, default, error, new[] { error });

    public static Result<T> Failure(IReadOnlyList<string> errors) =>
        new(false, default, errors.FirstOrDefault(), errors);
}
