using FluentValidation;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.CodeRunner;
using IQP.Infrastructure.Data;
using MediatR;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.AlgoTasks.RunCode;

public record RunCodeCommand : IRequest<TestRun>
{
    public Guid LanguageId { get; set; }
    public required string Code { get; set; }
    public required string Tests { get; set; }
}

public class RunCodeCommandHandler : IRequestHandler<RunCodeCommand, TestRun>
{
    private readonly ICodeLanguagesRepository _codeLanguagesRepository;
    private readonly ITestRunnerService _testRunner;
    private readonly IValidator<RunCodeCommand> _validator;
    
    public RunCodeCommandHandler(ICodeLanguagesRepository codeLanguagesRepository, ITestRunnerService testRunner, IValidator<RunCodeCommand> validator)
    {
        _codeLanguagesRepository = codeLanguagesRepository;
        _testRunner = testRunner;
        _validator = validator;
    }

    public async Task<TestRun> Handle(RunCodeCommand command, CancellationToken cancellationToken)
    {
        var commandValidationResult = _validator.Validate(command);

        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.AlgoTask, commandValidationResult.ToDictionary());
        }

        var language = await _codeLanguagesRepository.GetByIdAsync(command.LanguageId, cancellationToken);

        if (language is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}.{EntityName.CodeLanguage}", Errors.NotFound.ToString(),
                "CodeLanguage not found",
                "The code language with such id does not exist or is not supported. Therefore submission cannot be tested.");
        }

        var result = await _testRunner
            .RunTestsOnCode(
                command.Code,
                command.Tests,
                language.Slug,
                Guid.NewGuid().ToString());

        return result;
    }
}