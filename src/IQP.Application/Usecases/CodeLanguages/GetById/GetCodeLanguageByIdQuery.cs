using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using MediatR;

namespace IQP.Application.Usecases.CodeLanguages.GetById;

public record GetCodeLanguageByIdQuery : IRequest<CodeLanguageResponse>
{
    public required Guid Id { get; init; }
}

public class GetCodeLanguageByIdQueryHandler : IRequestHandler<GetCodeLanguageByIdQuery, CodeLanguageResponse>
{
    private readonly IqpDbContext _db;

    public GetCodeLanguageByIdQueryHandler(IqpDbContext db)
    {
        _db = db;
    }

    public async Task<CodeLanguageResponse> Handle(GetCodeLanguageByIdQuery request, CancellationToken cancellationToken)
    {
        var language = await _db.CodeLanguages.FindAsync(request.Id);
        if (language is null)
        {
            throw new IqpException(
                EntityName.CodeLanguage, Errors.NotFound.ToString(), "Not found",
                "The code language with such id does not exist.");
        }
        return language.ToResponse();
    }
}