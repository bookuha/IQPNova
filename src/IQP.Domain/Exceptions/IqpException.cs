namespace IQP.Domain.Exceptions;

public class IqpException : Exception
{
    public IqpException(EntityName entityName, string error, string title, string message) : base(message)
    {
        EntityName = entityName.ToString();
        Error = error;
        Title = title;
    }
    
    // String overload for EntityName to allow for custom entity names (e.g. subdomains)
    public IqpException(string entityName, string error, string title, string message) : base(message)
    {
        EntityName = entityName;
        Error = error;
        Title = title;
    }
    
    public string EntityName { get; }
    public string Error { get; }
    public string Title { get; }
    
    public static IqpException NotAdmin()
    {
        return new IqpException(Domain.EntityName.User, "Restricted" , "Forbidden", "You are not allowed to access this resource.");
    }
}