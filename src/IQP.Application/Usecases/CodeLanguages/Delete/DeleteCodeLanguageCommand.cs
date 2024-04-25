using IQP.Application.Contracts.CodeLanguages.Responses;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
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
    private readonly IqpDbContext _db;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;


    public DeleteCodeLanguageCommandHandler(IqpDbContext db, IUserService userService, ICurrentUserService currentUser)
    {
        _db = db;
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

        var language = await _db.CodeLanguages.FindAsync(request.Id);
        if (language is null)
        {
            throw new IqpException(
                EntityName.CodeLanguage, Errors.NotFound.ToString(), "Not found",
                "The code language with such id does not exist.");
        }

        var languageIsUsed = await _db.Database
            .SqlQueryRaw<Guid>("""SELECT "LanguageId" AS "Value" FROM "AlgoTaskCodeSnippet" """)
            .AnyAsync(lId => lId == request.Id);
        if (languageIsUsed)
        {
            throw new IqpException(
                EntityName.CodeLanguage, Errors.WrongFlow.ToString(), "Wrong flow",
                "The code language is used in some tasks. Therefore, it cannot be deleted.");
        }
        
        _db.CodeLanguages.Remove(language);
        await _db.SaveChangesAsync();
        return language.ToResponse();
    }
}