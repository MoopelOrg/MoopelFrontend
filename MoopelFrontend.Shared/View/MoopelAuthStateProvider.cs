using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;

using MoopelObjects.Dto.Read;

namespace MoopelFrontend.Shared.View;

/// <summary>
/// Exposes the current Moopel user to Blazor's authorization system
/// (AuthorizeView, AuthorizeRouteView, [Authorize]).
/// </summary>
public sealed class MoopelAuthStateProvider : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    public UserRead? CurrentUser { get; private set; }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(new AuthenticationState(CreatePrincipal(CurrentUser)));

    /// <summary>Sets or clears the signed-in user and notifies the authorization system.</summary>
    public void SetCurrentUser(UserRead? user)
    {
        CurrentUser = user;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static ClaimsPrincipal CreatePrincipal(UserRead? user)
    {
        if (user is null)
        {
            return Anonymous;
        }

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role)
        ];

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, AuthConstants.AuthenticationType));
    }
}
