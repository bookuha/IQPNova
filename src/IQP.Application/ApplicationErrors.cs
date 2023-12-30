namespace IQP.Application;

// Conventional error strings for exceptions. Later used for HTTP code resolution.
// Not in Domain like EntityNames because it also revolves around HTTP, pure application code, etc.
public enum Errors
{
    AlreadyExists,
    NotFound,
    Restricted,
    Critical,
    WrongFlow // Means there is a problem with business logic flow. Like user tries to like a question twice.
}