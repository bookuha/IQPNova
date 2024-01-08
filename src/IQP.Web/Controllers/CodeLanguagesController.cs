using IQP.Application.Contracts.CodeLanguages;
using IQP.Application.Contracts.CodeLanguages.Commands;
using IQP.Application.Contracts.CodeLanguages.Responses;
using IQP.Application.Services;
using IQP.Web.ViewModels.CodeLanguages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IQP.Web.Controllers;

[Route("api/code-languages")]
public class CodeLanguagesController : ControllerBase
{
    private readonly ICodeLanguagesService _codeLanguagesService;

    public CodeLanguagesController(ICodeLanguagesService codeLanguagesService)
    {
        _codeLanguagesService = codeLanguagesService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CodeLanguageResponse>> CreateLanguage([FromBody] CreateCodeLanguageRequest request)
    {
        var command = new CreateCodeLanguageCommand {Name = request.Name, Slug = request.Slug, Extension = request.Extension};

        var response = await _codeLanguagesService.CreateLanguage(command);

        return Created($"api/code-languages/{response.Id}", response);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CodeLanguageResponse>>> GetLanguages()
    {
        var response = await _codeLanguagesService.GetLanguages();

        return Ok(response);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CodeLanguageResponse>> GetLanguageById(Guid id)
    {
        var response = await _codeLanguagesService.GetLanguage(id);

        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<CodeLanguageResponse>> UpdateLanguage(Guid id, [FromBody] UpdateCodeLanguageRequest request)
    {
        var command = new UpdateCodeLanguageCommand {Id = id, Name = request.Name, Slug = request.Slug, Extension = request.Extension};

        var response = await _codeLanguagesService.UpdateLanguage(command);

        return Ok(response);
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<CodeLanguageResponse>> DeleteLanguage(Guid id)
    {
       return await _codeLanguagesService.DeleteLanguage(id);
    }
    



}