using FluentValidation;
using IQP.Application.Contracts.CodeLanguages;
using IQP.Application.Contracts.CodeLanguages.Commands;
using IQP.Application.Contracts.CodeLanguages.Responses;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Services;

public class CodeLanguagesService : ICodeLanguagesService
{
    private readonly IqpDbContext _db;
    private readonly IValidator<CreateCodeLanguageCommand> _createCommandValidator;
    private readonly IValidator<UpdateCodeLanguageCommand> _updateCommandValidator;
    
    public CodeLanguagesService(IqpDbContext db, IValidator<CreateCodeLanguageCommand> createCommandValidator, IValidator<UpdateCodeLanguageCommand> updateCommandValidator)
    {
        _db = db;
        _createCommandValidator = createCommandValidator;
        _updateCommandValidator = updateCommandValidator;
    }
    
    public async Task<CodeLanguageResponse> CreateLanguage(CreateCodeLanguageCommand command)
    {
        var validationResult = _createCommandValidator.Validate(command);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(EntityName.CodeLanguage, validationResult.ToDictionary());
        }
        
        var languageAlreadyExists = await _db.CodeLanguages
            .AnyAsync(l => 
            l.Name == command.Name || 
            l.Slug == command.Slug || 
            l.Extension == command.Extension);
        
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
        
        await _db.CodeLanguages.AddAsync(codeLanguage);
        await _db.SaveChangesAsync();
        return codeLanguage.ToResponse();
    }
    
    public async Task<IEnumerable<CodeLanguageResponse>> GetLanguages()
    {
        return await _db.CodeLanguages.Select(l => l.ToResponse()).ToListAsync();
    }
    
    public async Task<CodeLanguageResponse> GetLanguage(Guid id)
    {
        var language = await _db.CodeLanguages.FindAsync(id);
        if (language is null)
        {
            throw new IqpException(
                EntityName.CodeLanguage, Errors.NotFound.ToString(), "Not found",
                "The code language with such id does not exist.");
        }
        return language.ToResponse();
    }
    
    public async Task<CodeLanguageResponse> UpdateLanguage(UpdateCodeLanguageCommand command)
    {
        var validationResult = _updateCommandValidator.Validate(command);
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
    
    public async Task<CodeLanguageResponse> DeleteLanguage(Guid id)
    {
        var language = await _db.CodeLanguages.FindAsync(id);
        if (language is null)
        {
            throw new IqpException(
                EntityName.CodeLanguage, Errors.NotFound.ToString(), "Not found",
                "The code language with such id does not exist.");
        }

        var languageIsUsed = await _db.Database
            .SqlQueryRaw<Guid>("""SELECT "LanguageId" AS "Value" FROM "AlgoTaskCodeSnippet" """)
            .AnyAsync(lId => lId == id);
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