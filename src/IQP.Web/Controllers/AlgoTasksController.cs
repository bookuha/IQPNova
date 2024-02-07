using IQP.Application.Contracts.AlgoTasks.Commands;
using IQP.Application.Contracts.AlgoTasks.Responses;
using IQP.Application.Contracts.AlgoTasks.Utility;
using IQP.Application.Services;
using IQP.Infrastructure.CodeRunner;
using IQP.Web.ViewModels;
using IQP.Web.ViewModels.AlgoTasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IQP.Web.Controllers;

[Route("api/algo-tasks")]
public class AlgoTasksController : ControllerBase
{
    private readonly IAlgoTasksService _algoTasksService;
    
    public AlgoTasksController(IAlgoTasksService algoTasksService)
    {
        _algoTasksService = algoTasksService;
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
        
        var result = await _algoTasksService.CreateAlgoTask(command);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("{algoTaskId:guid}/languages")]
    public async Task<ActionResult<AlgoTaskResponse>> AddLanguage(Guid algoTaskId, [FromBody] InitialCodeSnippet request)
    {
        var command = new AddNewLanguageToAlgoTaskCommand
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
        
        var result = await _algoTasksService.AddNewLanguageToAlgoTask(command);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("{algoTaskId:guid}/submit")]
    public async Task<ActionResult<TestRun>> SubmitAlgoTaskSolution(Guid algoTaskId, [FromBody] SubmitCodeRequest request)
    {
        var command = new SubmitAlgoTaskSolutionCommand{ Code = request.Code, LanguageId = request.LanguageId, AlgoTaskId = algoTaskId };
        
        var result = await _algoTasksService.SubmitAlgoTaskSolution(command);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("{algoTaskId:guid}/test")]
    public async Task<ActionResult<TestRun>> TestsAlgoTaskSolution(Guid algoTaskId, [FromBody] SubmitCodeRequest request) 
    {
        var command = new SubmitAlgoTaskSolutionCommand{ Code = request.Code, LanguageId = request.LanguageId, AlgoTaskId = algoTaskId };
        
        var result = await _algoTasksService.TestAlgoTaskSolution(command);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("test")]
    public async Task<ActionResult<TestRun>> RunTestsOnCode([FromBody] RunTestsOnCodeRequest request)
    {
        var command = new RunTestsOnCodeCommand{ LanguageId = request.LanguageId, Code = request.Code, Tests = request.Tests };
        
        var result = await _algoTasksService.RunTestsOnCode(command);
        
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlgoTaskResponse>>> GetAlgoTasks()
    {
        var result = await _algoTasksService.GetAlgoTasks();
        
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AlgoTaskResponse>> GetAlgoTaskById(Guid id)
    {
        var result = await _algoTasksService.GetAlgoTaskById(id);
        
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
        
        var result = await _algoTasksService.UpdateAlgoTask(command);
        
        return Ok(result);
    }
}