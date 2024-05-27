using IQP.Domain.Exceptions;

namespace IQP.Domain.Entities.Questions;

public class Commentary
{
    public Guid Id { get; private set; }  = Guid.NewGuid(); // Note: See QuestionsRepository.AddCommentary
    public DateTime Created { get; private set; }
    
    public string Content { get; private set; } = null!;
    
    public Guid? ReplyToId { get; private set; }
    public Commentary? ReplyTo { get; private set; }
    public HashSet<Commentary> Replies { get; private set; } = new HashSet<Commentary>();

    public HashSet<User> LikedBy { get; private set; } = new HashSet<User>();

    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; }
    public Guid CreatedById { get; private set; }
    public User CreatedBy { get; private set; }
    
    public static Commentary Create(string content, Question question, User creator)
    {
        if (question is null)
        {
            throw new ArgumentException("Question cannot be null", nameof(question));
        }
        Validate(content, creator);
        
        return new Commentary
        {
            Content = content,
            Question = question,
            CreatedBy = creator,
        };
    }
    
    public void Update(string content)
    {
        Validate(content, CreatedBy);
        Content = content;
    }
    
    public Commentary Reply(string content, User creator)
    {
        Validate(content, creator);
        
        var reply = new Commentary
        {
            Content = content,
            Question = Question,
            CreatedBy = creator,
            ReplyTo = this,
        };

        Replies.Add(reply);
        return reply;
    }
    
    private static void Validate(string content, User creator)
    {
        var validationProblems = new Dictionary<string, string[]>();

        if (string.IsNullOrEmpty(content) || content.Length < 2 || content.Length > 2000)
        {
            validationProblems.Add("content", new[] {"Content must be between 2 and 2000 characters long and not empty."});
        }
        
        if (creator is null)
        {
            throw new ArgumentException("Creator cannot be null", nameof(creator));
        }

        if (validationProblems.Any())
        {
            throw new ValidationException(EntityName.Commentary, validationProblems);
        }
    }
}