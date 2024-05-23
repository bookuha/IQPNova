using FluentValidation;
using IQP.Application.Contracts.CodeLanguages.Responses;
using IQP.Application.Services;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Repositories;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.CodeLanguages.Create;

public record CreateCodeLanguageCommand : IRequest<CodeLanguageResponse>
{
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public required string Extension { get; init; }
}

public class CreateCodeLanguageCommandHandler : IRequestHandler<CreateCodeLanguageCommand, CodeLanguageResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICodeLanguagesRepository _codeLanguagesRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserService _userService;
    private readonly IValidator<CreateCodeLanguageCommand> _validator;

    public CreateCodeLanguageCommandHandler(IUnitOfWork unitOfWork, ICodeLanguagesRepository codeLanguagesRepository, ICurrentUserService currentUser, IUserService userService, IValidator<CreateCodeLanguageCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _codeLanguagesRepository = codeLanguagesRepository;
        _currentUser = currentUser;
        _userService = userService;
        _validator = validator;
    }

    public async Task<CodeLanguageResponse> Handle(CreateCodeLanguageCommand command, CancellationToken cancellationToken)
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
        
        var languageAlreadyExists = await _codeLanguagesRepository.ExistsAsync(command.Name, command.Slug, command.Extension, cancellationToken);
        
        if (languageAlreadyExists)
        {
            throw new IqpException(
                EntityName.CodeLanguage, Errors.AlreadyExists.ToString(), "Already exists",
                "The code language with such name, slug or extension already exists.");
        }
        
        var codeLanguage = new CodeLanguage
        {
            Name = command.Name,
            Slug = command.Slug,
            Extension = command.Extension
        };
        
        _codeLanguagesRepository.Add(codeLanguage);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return codeLanguage.ToResponse();
    }
}