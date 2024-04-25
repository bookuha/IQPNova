using FluentValidation;
using IQP.Application.Contracts.CodeLanguages.Responses;
using IQP.Application.Services;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.CodeLanguages.Update;

public class UpdateCodeLanguageCommand : IRequest<CodeLanguageResponse>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public required string Extension { get; init; }
}

public class UpdateCodeLanguageCommandHandler : IRequestHandler<UpdateCodeLanguageCommand, CodeLanguageResponse>
{
    private readonly IqpDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserService _userService;
    private readonly IValidator<UpdateCodeLanguageCommand> _validator;
    
    public UpdateCodeLanguageCommandHandler(IqpDbContext db, ICurrentUserService currentUser, IUserService userService, IValidator<UpdateCodeLanguageCommand> validator)
    {
        _db = db;
        _currentUser = currentUser;
        _userService = userService;
        _validator = validator;
    }

    public async Task<CodeLanguageResponse> Handle(UpdateCodeLanguageCommand command, CancellationToken cancellationToken)
    {
        if (!await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }

        var validationResult = _validator.Validate(command);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(EntityName.CodeLanguage, validationResult.ToDictionary());
        }
        
        var language = await _db.CodeLanguages.FindAsync(command.Id);
        if (language is null)
        {
            throw new IqpException(
                EntityName.CodeLanguage, Errors.NotFound.ToString(), "Not found",
                "The code language with such id does not exist.");
        }
        
        var languageAlreadyExists = await _db.CodeLanguages
            .AnyAsync(l => 
                l.Id != command.Id && 
                (l.Name == command.Name || 
                 l.Slug == command.Slug || 
                 l.Extension == command.Extension));

        if (languageAlreadyExists)
        {
            throw new IqpException(
                EntityName.CodeLanguage, Errors.AlreadyExists.ToString(), "Already exists",
                "The code language with such name, slug or extension already exists.");
        }
        
        language.Name = command.Name;
        language.Slug = command.Slug;
        language.Extension = command.Extension;
        
        await _db.SaveChangesAsync();
        return language.ToResponse();
    }
}