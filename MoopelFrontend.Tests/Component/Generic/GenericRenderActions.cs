namespace MoopelFrontend.Tests.Component.Generic;

/// <summary>
/// Shared bUnit render helpers used across component tests.
/// Mirrors GenericActions from MoopelBackend — provides reusable
/// building blocks so individual test classes stay focused on assertions.
/// </summary>
public static class GenericRenderActions
{
    // Add shared bUnit rendering helpers here as components are built.
    // Example pattern:
    //
    //   public static IRenderedComponent<T> RenderWithLayout<T>(
    //       TestContext ctx,
    //       params ComponentParameter[] parameters)
    //       where T : IComponent
    //   {
    //       return ctx.RenderComponent<T>(parameters);
    //   }
}
