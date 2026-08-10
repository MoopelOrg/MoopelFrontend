using MoopelFrontend.Client.Models;

namespace MoopelFrontend.Client.Api;

/// <summary>Typed access to MoopelBackend's note endpoints.</summary>
public interface INotesApiClient
{
    Task<ApiResult<List<Note>>> GetMyNotesAsync(CancellationToken cancellationToken = default);
    Task<ApiResult<Note>> CreateNoteAsync(NoteCreateRequest request, CancellationToken cancellationToken = default);
    Task<ApiResult<bool>> DeleteNoteAsync(int noteId, CancellationToken cancellationToken = default);
}

public sealed class NotesApiClient : INotesApiClient
{
    private readonly MoopelApiClient _api;

    public NotesApiClient(MoopelApiClient api)
    {
        _api = api;
    }

    public Task<ApiResult<List<Note>>> GetMyNotesAsync(CancellationToken cancellationToken = default)
        => _api.GetAsync<List<Note>>(ApiRoutes.Note.MyNotes, cancellationToken);

    public Task<ApiResult<Note>> CreateNoteAsync(NoteCreateRequest request, CancellationToken cancellationToken = default)
        => _api.PostAsync<Note>(ApiRoutes.Note.CreateNote, request, readErrorBody: false, cancellationToken);

    public Task<ApiResult<bool>> DeleteNoteAsync(int noteId, CancellationToken cancellationToken = default)
        => _api.DeleteAsync(ApiRoutes.Note.DeleteNote(noteId), cancellationToken);
}
