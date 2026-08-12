using MoopelFrontend.Shared;
using MoopelFrontend.Shared.Interfaces;
using MoopelFrontend.Shared.Models;

namespace MoopelFrontend.Client.Services;

/// <summary>Typed access to MoopelBackend's note endpoints.</summary>
public sealed class NotesService : INotesService
{
    private readonly IMoopelApiService _api;

    public NotesService(IMoopelApiService api)
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
