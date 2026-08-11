using Microsoft.AspNetCore.Components.Web;

namespace MoopelFrontend.Client;

/// <summary>
/// Shared render mode instances for routable pages. The @rendermode directive requires
/// a static reference, so render modes with custom options (like disabling prerender)
/// must be exposed this way rather than constructed inline.
/// </summary>
public static class RenderModes
{
    /// <summary>
    /// Interactive Auto without prerendering, so JS interop (e.g. localStorage-based
    /// auth token loading) is available as soon as the page's OnInitializedAsync runs.
    /// First visit uses Interactive Server; subsequent visits use WASM once cached.
    /// </summary>
    public static readonly InteractiveAutoRenderMode AutoNoPrerender = new(prerender: false);
}
