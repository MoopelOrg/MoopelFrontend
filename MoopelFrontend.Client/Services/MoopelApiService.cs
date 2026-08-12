using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Logging;

using MoopelFrontend.Shared.Interfaces;
using MoopelFrontend.Shared.Models;

namespace MoopelFrontend.Client.Services;

/// <summary>
/// The single place HTTP calls to MoopelBackend are made. Handles the base address,
/// bearer token attachment, JSON options, status-code mapping, and 401 sign-out —
/// so pages and feature services never deal with raw HTTP.
/// </summary>
public sealed class MoopelApiService : IMoopelApiService
{
    /// <summary>Matches MoopelApi's JSON configuration: camelCase + enums as strings.</summary>
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;
    private readonly ILogger<MoopelApiService> _logger;

    /// <summary>
    /// Invoked when an authenticated request comes back 401 (token expired/revoked).
    /// Set by AuthService so the whole app signs out consistently.
    /// </summary>
    public Func<Task>? OnUnauthorizedAsync { get; set; }

    public MoopelApiService(HttpClient httpClient, ITokenStore tokenStore, ILogger<MoopelApiService> logger)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
        _logger = logger;
    }

    public Task<ApiResult<T>> GetAsync<T>(string route, CancellationToken cancellationToken = default)
        => SendAsync<T>(HttpMethod.Get, route, body: null, readErrorBody: false, cancellationToken);

    public Task<ApiResult<T>> PostAsync<T>(string route, object? body,
        bool readErrorBody = false, CancellationToken cancellationToken = default)
        => SendAsync<T>(HttpMethod.Post, route, body, readErrorBody, cancellationToken);

    public Task<ApiResult<bool>> DeleteAsync(string route, CancellationToken cancellationToken = default)
        => SendAsync<bool>(HttpMethod.Delete, route, body: null, readErrorBody: false, cancellationToken);

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string route, object? body,
        bool readErrorBody, CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(method, route);

        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        string? token = _tokenStore.CurrentToken;
        bool wasAuthenticatedRequest = !string.IsNullOrWhiteSpace(token);
        if (wasAuthenticatedRequest)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network failure calling {Method} {Route}", method, route);
            return ApiResult<T>.Fail(ApiErrorKind.Network, "Could not reach the Moopel service.");
        }

        using (response)
        {
            if (response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(bool))
                {
                    return ApiResult<T>.Ok((T)(object)true);
                }

                T? value = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
                if (value is null)
                {
                    _logger.LogError("Empty response body from {Method} {Route}", method, route);
                    return ApiResult<T>.Fail(ApiErrorKind.Server, "The Moopel service returned an empty response.");
                }
                return ApiResult<T>.Ok(value);
            }

            return await MapFailureAsync<T>(response, method, route, wasAuthenticatedRequest, readErrorBody, cancellationToken);
        }
    }

    private async Task<ApiResult<T>> MapFailureAsync<T>(HttpResponseMessage response, HttpMethod method,
        string route, bool wasAuthenticatedRequest, bool readErrorBody, CancellationToken cancellationToken)
    {
        string rawBody = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogWarning("API failure {Status} from {Method} {Route}: {Body}",
            response.StatusCode, method, route, rawBody);

        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
                return ApiResult<T>.Fail(ApiErrorKind.Validation, CleanBodyMessage(rawBody));

            case HttpStatusCode.Unauthorized:
                T? errorValue = default;
                if (readErrorBody && !string.IsNullOrWhiteSpace(rawBody))
                {
                    errorValue = TryDeserialize<T>(rawBody);
                }

                // Only sign out when we thought we were authenticated —
                // a failed login attempt is not a session expiry.
                if (wasAuthenticatedRequest && OnUnauthorizedAsync is not null)
                {
                    await OnUnauthorizedAsync();
                }

                return ApiResult<T>.Fail(ApiErrorKind.Unauthorized, "Not authorized.", errorValue);

            case HttpStatusCode.Forbidden:
                return ApiResult<T>.Fail(ApiErrorKind.Forbidden, "You do not have access to that.");

            case HttpStatusCode.NotFound:
                return ApiResult<T>.Fail(ApiErrorKind.NotFound, "That item could not be found.");

            default:
                return ApiResult<T>.Fail(ApiErrorKind.Server, "The Moopel service ran into a problem.");
        }
    }

    private static T? TryDeserialize<T>(string rawBody)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(rawBody, JsonOptions);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    /// <summary>The backend returns BadRequest("message") bodies as JSON-quoted strings.</summary>
    private static string CleanBodyMessage(string rawBody)
    {
        string trimmed = rawBody.Trim();
        if (trimmed.Length >= 2 && trimmed.StartsWith('"') && trimmed.EndsWith('"'))
        {
            trimmed = trimmed[1..^1];
        }
        return string.IsNullOrWhiteSpace(trimmed) ? "The request was invalid." : trimmed;
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
