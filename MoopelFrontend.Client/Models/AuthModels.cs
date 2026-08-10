using System.ComponentModel.DataAnnotations;

namespace MoopelFrontend.Client.Models;

/// <summary>Mirrors MoopelApi's UserRead DTO.</summary>
public sealed record UserRead
{
    public required int UserId { get; init; }
    public required string Username { get; init; }
    public string? Email { get; init; }
    public required string Role { get; init; }

    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? LastLoginUtc { get; init; }

    public required bool Deactivated { get; init; }
}

/// <summary>Mirrors MoopelApi's LoginRequest. Data annotations drive the login form.</summary>
public sealed class LoginRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>Mirrors MoopelApi's RegistrationRequest. Data annotations drive the register form.
/// Username/password strength rules are enforced by the backend, which returns explanations.</summary>
public sealed class RegistrationRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Mirrors MoopelApi's LoginResult. The backend also returns Login/Session objects,
/// which the frontend does not need and ignores during deserialization.
/// </summary>
public sealed record LoginResult
{
    public string Token { get; init; } = string.Empty;
    public string? Error { get; init; }
    public UserRead? User { get; init; }
}
