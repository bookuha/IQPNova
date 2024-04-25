using IQP.Application.Usecases.Commentaries;
using IQP.Application.Usecases.Commentaries.Create;
using IQP.Application.Usecases.Commentaries.GetByQuestionId;
using IQP.Web.ViewModels.Questions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IQP.Web.Controllers;

[Route("api")]
[ApiController]
public class CommentariesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentariesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost("questions/{questionId}/commentaries")]
    public async Task<ActionResult<CommentaryResponse>> CreateCommentary(Guid questionId, [FromBody] CreateCommentaryRequest request)
    {
        var command = new CreateCommentaryCommand {QuestionId = questionId, Content = request.Content, ReplyToId = request.ReplyToId};

        var response = await _mediator.Send(command);

        return Created($"api/commentaries/{response.Id}", response);
    }
    
    
    [HttpGet("questions/{questionId}/commentaries")]
    public async Task<ActionResult<IEnumerable<CommentaryResponse>>> GetCommentariesByQuestionId(Guid questionId)
    {
        var result = await _mediator.Send(new GetCommentariesByQuestionIdQuery {QuestionId = questionId});

        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("commentaries/{id}")]
    public async Task<ActionResult<CommentaryResponse>> UpdateCommentary(Guid id, [FromBody] UpdateCommentaryRequest request)
    {
        throw new NotImplementedException();
    }
    
    [Authorize]
    [HttpDelete("commentaries/{id}")]
    public async Task<ActionResult<CommentaryResponse>> DeleteCommentary(Guid id)
    {
        throw new NotImplementedException();
    }
    
}