using System.ComponentModel.DataAnnotations;

using MoopelObjects.Enums;
using MoopelObjects.Requests.Creation;

namespace MoopelFrontend.Client.Models.Forms;

/// <summary>
/// Mutable, bindable counterpart to the immutable <see cref="NoteCreateRequest"/> contract.
/// </summary>
public sealed class NoteCreateForm
{
    public NoteType Type { get; set; } = NoteType.Other;

    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required.")]
    public string Content { get; set; } = string.Empty;

    public NoteCreateRequest ToRequest() => new()
    {
        Type = Type,
        Title = Title.Trim(),
        Content = Content
    };
}
