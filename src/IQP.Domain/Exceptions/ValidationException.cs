namespace IQP.Domain.Exceptions;

public class ValidationException : IqpException
{
    public ValidationException(EntityName name, IDictionary<string, string[]> errors)
        : base(
            name,
            "Validation",
            "One or more validation problems occured.", // TODO: -> "Validation Problem"?
            "One or more validation problems occured."
        )
    {
        Errors = errors;
    }
    
    // String overload for EntityName to allow for custom entity names (e.g. subdomains)
    public ValidationException(string name, IDictionary<string, string[]> errors)
        : base(
            name,
            "Validation",
            "One or more validation problems occured.", // TODO: -> "Validation Problem"?
            "One or more validation problems occured."
        )
    {
        Errors = errors;
    }
    
    public IDictionary<string, string[]> Errors { get;}
}