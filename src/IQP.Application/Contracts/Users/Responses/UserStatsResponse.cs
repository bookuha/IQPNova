using IQP.Domain.Entities;

namespace IQP.Application.Contracts.Users.Responses;

// Not used anywhere so far. Just a draft.
public class UserStatsResponse
{
    public UserStatsResponse()
    {
    }

    public required Guid Id { get; set; }
    public required string Nickname { get; set; }
    public required string Email { get; set; }
    public required UserStatus Status { get; set; }
    public required bool IsAdmin { get; set; }
    public required DateTime Registered { get; set; }
    public required int QuestionsCount { get; set; }
    public required int AnswersCount { get; set; }
    public required int CommentariesCount { get; set; }
    public required int LikesCount { get; set; }
}