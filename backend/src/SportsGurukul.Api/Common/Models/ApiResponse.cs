namespace SportsGurukul.Api.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public IReadOnlyList<string> Errors { get; set; } = [];

    public static ApiResponse<T> SuccessResult(T data, string message = "Success") =>
        new()
        {
            Success = true,
            Message = message,
            Data = data
        };

    public static ApiResponse<T> FailureResult(string error) =>
        new()
        {
            Success = false,
            Message = "Operation failed",
            Errors = new[] { error }
        };

    public static ApiResponse<T> FailureResult(IReadOnlyList<string> errors) =>
        new()
        {
            Success = false,
            Message = "Operation failed",
            Errors = errors
        };
}
