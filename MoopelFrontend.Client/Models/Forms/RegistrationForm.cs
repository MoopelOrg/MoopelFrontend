using System.ComponentModel.DataAnnotations;

using MoopelObjects.Requests;

namespace MoopelFrontend.Client.Models.Forms;

/// <summary>
/// Mutable, bindable counterpart to the immutable <see cref="RegistrationRequest"/> contract.
/// </summary>
public sealed class RegistrationForm
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

    public RegistrationRequest ToRequest() => new()
    {
        Username = Username.Trim(),
        Password = Password
    };
}
