using System.ComponentModel.DataAnnotations;

namespace MoopelFrontend.Client.Models;

/// <summary>Mirrors MoopelApi's NoteType enum. Serialized as strings by the backend.</summary>
public enum NoteType
{
    Storage,
    WorkItem,
    Fridge,
    Recipe,
    Other
}

/// <summary>Mirrors MoopelApi's Note DTO.</summary>
public sealed record Note
{
    public required int NoteId { get; init; }
    public required int UserId { get; init; }

    public required NoteType Type { get; init; }

    public required string Title { get; init; }
    public required string Content { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}

/// <summary>Mirrors MoopelApi's NoteCreateRequest. Data annotations drive the create form.</summary>
public sealed class NoteCreateRequest
{
    public NoteType Type { get; set; } = NoteType.Other;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}
