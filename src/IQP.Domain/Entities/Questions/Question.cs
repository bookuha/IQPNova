using IQP.Application;
using IQP.Domain.Exceptions;

namespace IQP.Domain.Entities.Questions;

public class Question
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime Created { get; private set; }
    
    public string Title { get; private set; } = null!;
    public string Description { get; private set; }  = null!;

    public HashSet<Commentary> Commentaries { get; private set; } = new HashSet<Commentary>();
    public HashSet<User> LikedBy { get; private set; } = new HashSet<User>();
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public Guid CreatorId { get; private set; }
    public User Creator { get; private set; }
    
    public static Question Create(string title, string description, Category category, User creator)
    {
        Validate(title, description, category);
        
        return new Question
        {
            Title = title,
            Description = description,
            Category = category,
            Creator = creator
        };
    }
    
    public void Update(string title, string description, Category category, User updater)
    {
        if (updater.Id != Creator.Id && !updater.IsAdmin)
        {
            throw new IqpException(
                EntityName.Question, Errors.Restricted.ToString(), "Restricted",
                "You are not allowed to update this question.");
        }
        Validate(title, description, category);
        
        Title = title;
        Description = description;
        Category = category;
    }
    
    // Add delete method.

    public Commentary Comment(string content, User creator, Guid? replyToId = null)
    {
        if (replyToId is not null) // Means it is not a root commentary
        {
            var root = Commentaries.SingleOrDefault(c => c.Id == replyToId);

            if (root is null)
            {
                throw new IqpException(
                    EntityName.Commentary, Errors.NotFound.ToString(), "Commentary not found",
                    "The root commentary with such id does not exist. Therefore commentary cannot be created.");
            }
            // Note: I want to allow only 1 level of replies.
            // Therefore, If specified commentary is a reply itself, then redirect the new commentary to its root.

            var intendedRootId = root.ReplyToId ?? root.Id;
            
            var intendedRootCommentary = Commentaries.SingleOrDefault(c => c.Id == intendedRootId);

            var reply = intendedRootCommentary.Reply(content, creator);
            // Commentaries.Add(reply); // 5/24/24 9:25 - to pass tests. Not yet tested with EF.
            return reply;
        }
        
        var commentary = Commentary.Create(content, this, creator);
        Commentaries.Add(commentary);
        return commentary;
    }
    
    public void Like(User user)
    {
        var isLikedAlready = LikedBy.Any(u=>u.Id == user.Id);
        
        if (isLikedAlready)
        {
            LikedBy.Remove(user);
        }
        else
        {
            LikedBy.Add(user);
        }
    }
    
    private static void Validate(string title, string description, Category category)
    {
        var validationProblems = new Dictionary<string, string[]>();

        if (string.IsNullOrEmpty(title) || title.Length < 10 || title.Length > 100)
        {
            validationProblems.Add("title", new[] {"Title must be between 10 and 100 characters long and not empty."});
        }

        if (string.IsNullOrEmpty(description) || description.Length < 20 || description.Length > 600)
        {
            validationProblems.Add("description",
                new[] {"Description must be between 20 and 600 characters long and not empty."});
        }

        if (category is null)
        {
            validationProblems.Add("category", new[] {"Category cannot be null."});
        }

        if (validationProblems.Any())
        {
            throw new ValidationException(EntityName.Question, validationProblems);
        }
    }   
}