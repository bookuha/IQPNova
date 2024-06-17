using IQP.Domain.Entities.AlgoTasks;
using IQP.Domain.Entities.Questions;
using Microsoft.AspNetCore.Identity;

namespace IQP.Domain.Entities;

public enum UserStatus
{
    SelfLearning,
    Student,
    Working
}

public class User : IdentityUser<Guid>
{
    // All the needed properties are inherited from the IdentityUser
    public UserStatus Status { get; set; }
    public bool IsAdmin { get; set; }
    public HashSet<Commentary> LikedCommentaries { get; set; } = new HashSet<Commentary>();
    public HashSet<Question> LikedQuestions { get; set; } = new HashSet<Question>();
    public HashSet<AlgoTask> LikedAlgoTasks { get; set; } = new HashSet<AlgoTask>();
    public HashSet<Commentary> CreatedCommentaries { get; set; } = new HashSet<Commentary>();
    public HashSet<Question> CreatedQuestions { get; set; } = new HashSet<Question>();
    public HashSet<TechTask> CreatedTechTasks { get; set; } = new HashSet<TechTask>();
    public HashSet<TechTaskSubmission> TechTaskSubmissions { get; set; } = new HashSet<TechTaskSubmission>();
    public HashSet<AlgoTask> PassedAlgoTasks { get; set; } = new HashSet<AlgoTask>();
}