using IQP.Application.Contracts.Commentaries;
using IQP.Application.Contracts.Commentaries.Commands;

namespace IQP.Application.Services;

public interface ICommentariesService
{
    public Task<CommentaryResponse> CreateCommentary(CreateCommentaryCommand command);
    public Task<IEnumerable<CommentaryResponse>> GetCommentariesByQuestionId(Guid questionId);
    public Task<CommentaryResponse> UpdateCommentary(UpdateCommentaryCommand command);
    public Task<CommentaryResponse> DeleteCommentary(Guid id);
}