using MoopelFrontend.Shared.Models;

using MoopelObjects.Dto;
using MoopelObjects.Requests.Creation;

namespace MoopelFrontend.Shared.Services.Interfaces;

public interface INotesService
{
    Task<ApiResult<List<Note>>> GetMyNotesAsync(CancellationToken cancellationToken = default);
    Task<ApiResult<Note>> CreateNoteAsync(NoteCreateRequest request, CancellationToken cancellationToken = default);
    Task<ApiResult<bool>> DeleteNoteAsync(int noteId, CancellationToken cancellationToken = default);
}
