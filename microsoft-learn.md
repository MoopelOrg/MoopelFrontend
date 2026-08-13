# Microsoft Learn Documentation


## Blazor Auth
https://learn.microsoft.com/en-us/aspnet/core/blazor/security/?view=aspnetcore-10.0&tabs=visual-studio

Razor Pages authorization conventions don't apply to routable Razor components. If a non-routable Razor component is embedded in a page of a Razor Pages app, the page's authorization conventions indirectly affect the Razor component along with the rest of the page's content.

I don't know what this is Cross-Site Request Forgery (CSRF/XSRF)

The [Authorize] attribute also supports role-based or policy-based authorization. For role-based authorization, use the AuthorizeAttribute.Roles parameter:
 - @attribute [Authorize(Roles = "Admin, Superuser")]

For policy-based authorization, use the Policy parameter:
 - @attribute [Authorize(Policy = "Over21")]

## App Secrets
https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-10.0&tabs=windows%2Cpowershell