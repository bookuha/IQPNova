using FluentValidation;
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

namespace IQP.Application.Usecases.Questions.Create;

public class CreateQuestionCommand : IRequest<QuestionResponse>
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid CategoryId { get; set; }
}

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, QuestionResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateQuestionCommandHandler> _logger;
    private readonly IValidator<CreateQuestionCommand> _validator;
    
    public CreateQuestionCommandHandler(IUnitOfWork unitOfWork, IQuestionsRepository questionsRepository, ICategoriesRepository categoriesRepository, IUserService userService, ICurrentUserService currentUser, ILogger<CreateQuestionCommandHandler> logger, IValidator<CreateQuestionCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _questionsRepository = questionsRepository;
        _categoriesRepository = categoriesRepository;
        _userService = userService;
        _currentUser = currentUser;
        _logger = logger;
        _validator = validator;
    }

    public async Task<QuestionResponse> Handle(CreateQuestionCommand command, CancellationToken cancellationToken)
    {
        var commandValidationResult = _validator.Validate(command);

        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.Question, commandValidationResult.ToDictionary());
        }

        var category = await _categoriesRepository.GetByIdAsync(command.CategoryId, cancellationToken);
        
        if (category is null)
        {
            throw new IqpException(
                $"{EntityName.Question}.{EntityName.Category}", Errors.NotFound.ToString(), "Category not found",
                "The category with such id does not exist. Therefore question cannot be created.");
        }
        
        var user = await _userService.GetUserByIdAsync(_currentUser.UserId.Value);

        var question = new Question
        {
            Title = command.Title,
            Description = command.Description,
            Creator = user,
            CategoryId = command.CategoryId
        };

        _questionsRepository.Add(question);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Question with id {QuestionId}, {Title} has been created", question.Id, question.Title);

        return question.ToResponse();
    }
}