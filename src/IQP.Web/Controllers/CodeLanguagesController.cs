using IQP.Application.Contracts.CodeLanguages;
using IQP.Application.Contracts.CodeLanguages.Responses;
using IQP.Application.Services;
using IQP.Application.Usecases.CodeLanguages.Create;
using IQP.Application.Usecases.CodeLanguages.Delete;
using IQP.Application.Usecases.CodeLanguages.Get;
using IQP.Application.Usecases.CodeLanguages.GetById;
using IQP.Application.Usecases.CodeLanguages.Update;
using IQP.Web.ViewModels.CodeLanguages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IQP.Web.Controllers;

[Route("api/code-languages")]
public class CodeLanguagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CodeLanguagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult<CodeLanguageResponse>> CreateLanguage([FromBody] CreateCodeLanguageRequest request)
    {
        var command = new CreateCodeLanguageCommand {Name = request.Name, Slug = request.Slug, Extension = request.Extension};

        var response = await _mediator.Send(command);

        return Created($"api/code-languages/{response.Id}", response);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CodeLanguageResponse>>> GetLanguages()
    {
        var response = await _mediator.Send(new GetCodeLanguagesQuery());
        
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CodeLanguageResponse>> GetLanguageById(Guid id)
    {
        var response = await _mediator.Send(new GetCodeLanguageByIdQuery {Id = id});

        return Ok(response);
    }
    
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<ActionResult<CodeLanguageResponse>> UpdateLanguage(Guid id, [FromBody] UpdateCodeLanguageRequest request)
    {
        var command = new UpdateCodeLanguageCommand {Id = id, Name = request.Name, Slug = request.Slug, Extension = request.Extension};

        var response = await _mediator.Send(command);

        return Ok(response);
    }
    
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<CodeLanguageResponse>> DeleteLanguage(Guid id)
    {
       return await _mediator.Send(new DeleteCodeLanguageCommand {Id = id});
    }
    



}