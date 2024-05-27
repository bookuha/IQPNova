using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Repositories;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.CodeLanguages.Delete;

public record DeleteCodeLanguageCommand : IRequest<CodeLanguageResponse>
{
    public required Guid Id { get; init; }
}

public class DeleteCodeLanguageCommandHandler : IRequestHandler<DeleteCodeLanguageCommand, CodeLanguageResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICodeLanguagesRepository _codeLanguagesRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    
    public DeleteCodeLanguageCommandHandler(IUnitOfWork unitOfWork, ICodeLanguagesRepository codeLanguagesRepository, IUserService userService, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _codeLanguagesRepository = codeLanguagesRepository;
        _userService = userService;
        _currentUser = currentUser;
    }

    public async Task<CodeLanguageResponse> Handle(DeleteCodeLanguageCommand request,
        CancellationToken cancellationToken)
    {
        if (!await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }

        var language = await _codeLanguagesRepository.GetByIdAsync(request.Id, cancellationToken);
        if (language is null)
        {
            throw new IqpException(
                EntityName.CodeLanguage, Errors.NotFound.ToString(), "Not found",
                "The code language with such id does not exist.");
        }

        var languageIsUsed = await _codeLanguagesRepository.IsInUseAsync(language.Id, cancellationToken);
        if (languageIsUsed)
        {
            throw new IqpException(
                EntityName.CodeLanguage, Errors.WrongFlow.ToString(), "Wrong flow",
                "The code language is used in some tasks. Therefore, it cannot be deleted.");
        }
        
        _codeLanguagesRepository.Delete(language);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return language.ToResponse();
    }
}