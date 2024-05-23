using System.ComponentModel.DataAnnotations;
using IQP.Application.Usecases.AlgoTasks;
using IQP.Application.Usecases.AlgoTasks.Create;
using IQP.Application.Usecases.AlgoTasks.Get;
using IQP.Application.Usecases.AlgoTasks.GetById;
using IQP.Application.Usecases.AlgoTasks.RunCode;
using IQP.Application.Usecases.AlgoTasks.SubmitSolution;
using IQP.Application.Usecases.AlgoTasks.Translate;
using IQP.Application.Usecases.AlgoTasks.TrySolution;
using IQP.Application.Usecases.AlgoTasks.Update;
using IQP.Infrastructure.CodeRunner;
using IQP.Web.ViewModels.AlgoTasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IQP.Web.Controllers;

[Route("api/algo-tasks")]
public class AlgoTasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public AlgoTasksController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult<AlgoTaskResponse>> CreateAlgoTask([FromBody] CreateAlgoTaskRequest request)
    {
        var command = new CreateAlgoTaskCommand
        {
            Title = request.Title,
            Description = request.Description,
            AlgoCategoryId = request.AlgoCategoryId,
            InitialCodeSnippet = new CodeSnippet
            {
                LanguageId = request.InitialCodeSnippet.LanguageId,
                InitialSolutionCode = request.InitialCodeSnippet.InitialSolutionCode,
                TestsCode = request.InitialCodeSnippet.TestsCode,
                SampleCode = request.InitialCodeSnippet.SampleCode
            }
        };
        
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("{algoTaskId:guid}/languages")]
    public async Task<ActionResult<AlgoTaskResponse>> AddLanguage(Guid algoTaskId, [FromBody] InitialCodeSnippet request)
    {
        var command = new TranslateAlgoTaskCommand
        {
            AlgoTaskId = algoTaskId,
            InitialCodeSnippet = new CodeSnippet
            {
                LanguageId = request.LanguageId,
                InitialSolutionCode = request.InitialSolutionCode,
                TestsCode = request.TestsCode,
                SampleCode = request.SampleCode
            }
        };
        
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("{algoTaskId:guid}/submit")]
    public async Task<ActionResult<TestRun>> SubmitAlgoTaskSolution(Guid algoTaskId, [FromBody] SubmitCodeRequest request)
    {
        var command = new SubmitAlgoTaskSolutionCommand{ Code = request.Code, LanguageId = request.LanguageId, AlgoTaskId = algoTaskId };
        
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("{algoTaskId:guid}/try")]
    public async Task<ActionResult<TestRun>> TryAlgoTaskSolution(Guid algoTaskId, [FromBody] SubmitCodeRequest request) 
    {
        var command = new TryAlgoTaskSolutionCommand{ Code = request.Code, LanguageId = request.LanguageId, AlgoTaskId = algoTaskId };
        
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("run-code")]
    public async Task<ActionResult<TestRun>> RunTestsOnCode([FromBody] RunTestsOnCodeRequest request)
    {
        var command = new RunCodeCommand{ LanguageId = request.LanguageId, Code = request.Code, Tests = request.Tests };
        
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlgoTaskResponse>>> GetAlgoTasks(string? searchTerm, Guid? algoCategoryId,
        string? sortColumn, string? sortOrder, [Range(1, int.MaxValue)] int page = 1, [Range(1,30)] int pageSize = 15)
    {
        var result =
            await _mediator.Send(new GetAlgoTasksQuery(searchTerm, algoCategoryId, sortColumn, sortOrder, page, pageSize));

        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AlgoTaskResponse>> GetAlgoTaskById(Guid id)
    {
        var result = await _mediator.Send(new GetAlgoTaskByIdQuery{ Id = id });
        
        return Ok(result);
    }
    
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{algoTaskId:guid}")]
    public async Task<ActionResult<AlgoTaskResponse>> UpdateAlgoTask(Guid algoTaskId, [FromBody] UpdateAlgoTaskRequest request)
    {
        var command = new UpdateAlgoTaskCommand
        {
            Id = algoTaskId,
            Title = request.Title,
            Description = request.Description,
            AlgoCategoryId = request.AlgoCategoryId
        };
        
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }
}