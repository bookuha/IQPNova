using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.CodeLanguages.Get;

public record GetCodeLanguagesQuery : IRequest<IEnumerable<CodeLanguageResponse>>
{
    
}

public class GetCodeLanguagesQueryHandler : IRequestHandler<GetCodeLanguagesQuery, IEnumerable<CodeLanguageResponse>>
{
    private readonly ICodeLanguagesRepository _codeLanguagesRepository;

    public GetCodeLanguagesQueryHandler(ICodeLanguagesRepository codeLanguagesRepository)
    {
        _codeLanguagesRepository = codeLanguagesRepository;
    }

    public async Task<IEnumerable<CodeLanguageResponse>> Handle(GetCodeLanguagesQuery request, CancellationToken cancellationToken)
    {
        var languages = await _codeLanguagesRepository.GetAsync(cancellationToken);

        return languages.Select(l => l.ToResponse());
    }
}