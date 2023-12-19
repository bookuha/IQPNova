using IQP.Application.Contracts.Commentaries;
using IQP.Application.Contracts.Commentaries.Commands;
using IQP.Application.Services;
using IQP.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IQP.Web.Controllers;

[Route("api")]
[ApiController]
public class CommentariesController : ControllerBase
{
    private readonly CommentariesService _commentariesService;

    public CommentariesController(CommentariesService commentariesService)
    {
        _commentariesService = commentariesService;
    }

    [Authorize]
    [HttpPost("questions/{questionId}/commentaries")]
    public async Task<ActionResult<CommentaryResponse>> CreateCommentary(Guid questionId, [FromBody] CreateCommentaryRequest request)
    {
        var command = new CreateCommentaryCommand {QuestionId = questionId, Content = request.Content, ReplyToId = request.ReplyToId};

        var response = await _commentariesService.CreateCommentary(command);

        return Created($"api/commentaries/{response.Id}", response);
    }
    
    
    [HttpGet("questions/{questionId}/commentaries")]
    public async Task<ActionResult<IEnumerable<CommentaryResponse>>> GetCommentariesByQuestionId(Guid questionId)
    {
        var result = await _commentariesService.GetCommentariesByQuestionId(questionId);

        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("commentaries/{id}")]
    public async Task<ActionResult<CommentaryResponse>> UpdateCommentary(Guid id, [FromBody] UpdateCommentaryRequest request)
    {
        var command = new UpdateCommentaryCommand {Id = id, Content = request.Content};

        var result = await _commentariesService.UpdateCommentary(command);

        return Ok(result);
    }
    
    [Authorize]
    [HttpDelete("commentaries/{id}")]
    public async Task<ActionResult<CommentaryResponse>> DeleteCommentary(Guid id)
    {
        var result = await _commentariesService.DeleteCommentary(id);

        return Ok(result);
    }
    
}