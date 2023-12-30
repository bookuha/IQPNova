using IQP.Application.Contracts.Questions.Commands;
using IQP.Application.Contracts.Questions.Responses;
using IQP.Application.Services;
using IQP.Web.ViewModels;
using IQP.Web.ViewModels.Questions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IQP.Web.Controllers;

[Route("api/questions")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly QuestionsService _questionsService;

    public QuestionsController(QuestionsService questionsService)
    {
        _questionsService = questionsService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<QuestionResponse>> CreateQuestion([FromBody] CreateQuestionRequest request)
    {
        var command = new CreateQuestionCommand {Title = request.Title, Description = request.Description, CategoryId = request.CategoryId};

        var response = await _questionsService.CreateQuestion(command);

        return Created($"api/questions/{response.Id}", response);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionResponse>> GetQuestionById(Guid id)
    {
        var result = await _questionsService.GetQuestionById(id);

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestionResponse>>> GetQuestions()
    {
        var result = await _questionsService.GetQuestions();

        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<QuestionResponse>> UpdateQuestion(Guid id, [FromBody] UpdateQuestionRequest request)
    {
        var command = new UpdateQuestionCommand {Id = id, Title = request.Title, Description = request.Description, CategoryId = request.CategoryId};

        var result = await _questionsService.UpdateQuestion(command);

        return Ok(result);
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<QuestionResponse>> DeleteQuestion(Guid id)
    {
        var result = await _questionsService.DeleteQuestion(id);

        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("{id}/likes")]
    public async Task<ActionResult<QuestionResponse>> LikeQuestion(Guid id)
    {
        var result = await _questionsService.LikeQuestion(id);

        return Ok(result);
    }
}