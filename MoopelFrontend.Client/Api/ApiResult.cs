namespace MoopelFrontend.Client.Api;

/// <summary>Categorizes API failures so the UI can react consistently.</summary>
public enum ApiErrorKind
{
    None,
    Validation,
    Unauthorized,
    Forbidden,
    NotFound,
    Server,
    Network
}

/// <summary>
/// Result of an API call. On failure, <see cref="ErrorKind"/> and <see cref="Message"/>
/// describe what went wrong. <see cref="Value"/> may still be populated on failure when
/// the backend returns a body with error details (e.g. login failures).
/// </summary>
public sealed record ApiResult<T>
{
    public bool Success { get; init; }
    public T? Value { get; init; }
    public ApiErrorKind ErrorKind { get; init; } = ApiErrorKind.None;
    public string? Message { get; init; }

    public static ApiResult<T> Ok(T value) => new()
    {
        Success = true,
        Value = value
    };

    public static ApiResult<T> Fail(ApiErrorKind kind, string? message, T? value = default) => new()
    {
        Success = false,
        ErrorKind = kind,
        Message = message,
        Value = value
    };
}
