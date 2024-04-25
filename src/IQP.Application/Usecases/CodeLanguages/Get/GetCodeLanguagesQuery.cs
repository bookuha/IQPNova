using IQP.Application.Contracts.CodeLanguages.Responses;
using IQP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.CodeLanguages.Get;

public record GetCodeLanguagesQuery : IRequest<IEnumerable<CodeLanguageResponse>>
{
    
}

public class GetCodeLanguagesQueryHandler : IRequestHandler<GetCodeLanguagesQuery, IEnumerable<CodeLanguageResponse>>
{
    private readonly IqpDbContext _db;
    
    public GetCodeLanguagesQueryHandler(IqpDbContext db)
    {
        _db = db;
    }
    
    public async Task<IEnumerable<CodeLanguageResponse>> Handle(GetCodeLanguagesQuery request, CancellationToken cancellationToken)
    {
        return await _db.CodeLanguages.Select(l => l.ToResponse()).ToListAsync();
    }
}