using Bunit;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using MoopelFrontend.Client.View.Pages;
using MoopelFrontend.Shared.Models;
using MoopelFrontend.Shared.Services.Interfaces;
using MoopelFrontend.Tests.TestData;

using MoopelObjects.Dto;
using MoopelObjects.Dto.Read;
using MoopelObjects.Requests.Creation;

namespace MoopelFrontend.Tests.Component;

public class TagsTests
{
    [Fact]
    public void Tags_RendersSampleRowsAndCreateLink_WhenAuthenticated()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetAuthorized("testuser");
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService { CurrentUser = TestUser() });

        // Act
        IRenderedComponent<Tags> cut = ctx.Render<Tags>();

        // Assert
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(4, cut.FindAll(".tags-table-row").Count);
            Assert.Equal($"{TestData.PageRoutes.NewTag}?{PageRoutes.ReturnUrlParameter}=%2Ftags", cut.Find(".tags-create-link").GetAttribute("href"));
        });
    }

    [Fact]
    public void NewTag_CreateReturnsToSuppliedReturnUrl_WhenAuthenticated()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetAuthorized("testuser");
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService { CurrentUser = TestUser() });
        NavigationManager navigation = ctx.Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo($"{TestData.PageRoutes.NewTag}?{PageRoutes.ReturnUrlParameter}=%2Fnotes");
        IRenderedComponent<NewTag> cut = ctx.Render<NewTag>();

        // Act
        cut.Find(".tag-editor-submit").Click();

        // Assert
        Assert.Contains(TestData.PageRoutes.Notes, navigation.Uri, StringComparison.Ordinal);
    }

    [Fact]
    public void Notes_RendersCreateTagEntryPoint_WithReusableReturnUrl()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetAuthorized("testuser");
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService { CurrentUser = TestUser() });
        ctx.Services.AddSingleton<INotesService>(new FakeNotesService());

        // Act
        IRenderedComponent<Notes> cut = ctx.Render<Notes>();

        // Assert
        cut.WaitForAssertion(() =>
            Assert.Equal($"{TestData.PageRoutes.NewTag}?{PageRoutes.ReturnUrlParameter}=%2Fnotes", cut.Find(".note-tags-create").GetAttribute("href")));
    }

    private static UserRead TestUser() => new()
    {
        UserId = 42,
        Username = "testuser",
        Email = "test@example.com",
        Role = "Standard",
        CreatedAtUtc = DateTime.UtcNow,
        Deactivated = false
    };

    private sealed class FakeNotesService : INotesService
    {
        public Task<ApiResult<List<Note>>> GetMyNotesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(ApiResult<List<Note>>.Ok([]));

        public Task<ApiResult<Note>> CreateNoteAsync(NoteCreateRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(ApiResult<Note>.Fail(ApiErrorKind.Validation, "Not implemented."));

        public Task<ApiResult<bool>> DeleteNoteAsync(int noteId, CancellationToken cancellationToken = default)
            => Task.FromResult(ApiResult<bool>.Ok(true));
    }
}
