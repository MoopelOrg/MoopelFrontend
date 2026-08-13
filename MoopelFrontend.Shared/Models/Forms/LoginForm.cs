using System.ComponentModel.DataAnnotations;

using MoopelObjects.Requests;

namespace MoopelFrontend.Shared.Models.Forms;

/// <summary>
/// Mutable, bindable counterpart to the immutable <see cref="LoginRequest"/> contract.
/// </summary>
public sealed class LoginForm
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

    public LoginRequest ToRequest() => new()
    {
        Username = Username.Trim(),
        Password = Password
    };
}
