﻿using FluentValidation;
using IQP.Application.Contracts.AlgoTaskCategories.Responses;
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
using Microsoft.Extensions.Logging;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.AlgoCategories.Create;

public record CreateAlgoTaskCategoryCommand : IRequest<AlgoTaskCategoryResponse>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
}

public class CreateAlgoTaskCategoryCommandHandler : IRequestHandler<CreateAlgoTaskCategoryCommand, AlgoTaskCategoryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAlgoCategoriesRepository _algoCategoriesRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateAlgoTaskCategoryCommand> _createAlgoTaskCategoryCommandValidator;
    private readonly ILogger<CreateAlgoTaskCategoryCommandHandler> _logger;
    
    public CreateAlgoTaskCategoryCommandHandler(IUnitOfWork unitOfWork, IAlgoCategoriesRepository algoCategoriesRepository, IUserService userService, ICurrentUserService currentUser, IValidator<CreateAlgoTaskCategoryCommand> createAlgoTaskCategoryCommandValidator, ILogger<CreateAlgoTaskCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _algoCategoriesRepository = algoCategoriesRepository;
        _userService = userService;
        _currentUser = currentUser;
        _createAlgoTaskCategoryCommandValidator = createAlgoTaskCategoryCommandValidator;
        _logger = logger;
    }

    public async Task<AlgoTaskCategoryResponse> Handle(CreateAlgoTaskCategoryCommand command, CancellationToken cancellationToken)
    {
        if (!await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }

        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var commandValidationResult = _createAlgoTaskCategoryCommandValidator.Validate(command);

        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.AlgoCategory, commandValidationResult.ToDictionary());
        }
        
        var titleAlreadyExists = await _algoCategoriesRepository.TitleExistsAsync(command.Title, cancellationToken);

        if (titleAlreadyExists)
        {
            throw new IqpException(
                EntityName.AlgoCategory,Errors.AlreadyExists.ToString(), "Already exists", "The category with such title already exists.");
        }

        var category = new AlgoTaskCategory
        {
            Title = command.Title,
            Description = command.Description
        };

        _algoCategoriesRepository.Add(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Category with id {CategoryId}, {Title} has been created", category.Id, category.Title);

        return category.ToResponse();
        
        
    }
}