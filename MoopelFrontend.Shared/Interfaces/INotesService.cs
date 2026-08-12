using MoopelFrontend.Shared.Models;

namespace MoopelFrontend.Shared.Interfaces;

public interface INotesService
{
    Task<ApiResult<List<Note>>> GetMyNotesAsync(CancellationToken cancellationToken = default);
    Task<ApiResult<Note>> CreateNoteAsync(NoteCreateRequest request, CancellationToken cancellationToken = default);
    Task<ApiResult<bool>> DeleteNoteAsync(int noteId, CancellationToken cancellationToken = default);
}
