using System.ComponentModel.DataAnnotations;
using IQP.Application.Usecases.Questions;
using IQP.Application.Usecases.Questions.Create;
using IQP.Application.Usecases.Questions.Delete;
using IQP.Application.Usecases.Questions.Get;
using IQP.Application.Usecases.Questions.GetById;
using IQP.Application.Usecases.Questions.Like;
using IQP.Application.Usecases.Questions.Update;
using IQP.Web.ViewModels.Questions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IQP.Web.Controllers;

[Route("api/questions")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuestionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<QuestionResponse>> CreateQuestion([FromBody] CreateQuestionRequest request)
    {
        var command = new CreateQuestionCommand {Title = request.Title, Description = request.Description, CategoryId = request.CategoryId};

        var response = await _mediator.Send(command);

        return Created($"api/questions/{response.Id}", response);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionResponse>> GetQuestionById(Guid id)
    {
        var result = await _mediator.Send(new GetQuestionByIdQuery {Id = id});

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestionResponse>>> GetQuestions(string? searchTerm, Guid? categoryId,
        string? sortColumn, string? sortOrder, [Range(1, int.MaxValue)] int page = 1, [Range(1,30)] int pageSize = 15)
    {
        var result =
            await _mediator.Send(new GetQuestionsQuery(searchTerm, categoryId, sortColumn, sortOrder, page, pageSize));

        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<QuestionResponse>> UpdateQuestion(Guid id, [FromBody] UpdateQuestionRequest request)
    {
        var command = new UpdateQuestionCommand {Id = id, Title = request.Title, Description = request.Description, CategoryId = request.CategoryId};

        var result = await _mediator.Send(command);

        return Ok(result);
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<QuestionResponse>> DeleteQuestion(Guid id)
    {
        var result = await _mediator.Send(new DeleteQuestionCommand {Id = id});

        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("{id}/likes")]
    public async Task<ActionResult<QuestionResponse>> LikeQuestion(Guid id)
    {
        var result = await _mediator.Send(new LikeQuestionCommand {Id = id});

        return Ok(result);
    }
}