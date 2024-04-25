using IQP.Application.Services;
using IQP.Application.Usecases.Categories;
using IQP.Application.Usecases.Categories.Create;
using IQP.Application.Usecases.Categories.Delete;
using IQP.Application.Usecases.Categories.Get;
using IQP.Application.Usecases.Categories.GetById;
using IQP.Application.Usecases.Categories.Update;
using IQP.Web.ViewModels;
using IQP.Web.ViewModels.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IQP.Web.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var command = new CreateCategoryCommand {Title = request.Title, Description = request.Description};

        var response = await _mediator.Send(command);

        return Created($"api/categories/{response.Id}", response);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponse>> GetCategoryById(Guid id)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery {Id = id});

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories()
    {
        var result = await _mediator.Send(new GetCategoriesQuery());

        return Ok(result);
    }
    
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryResponse>> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var command = new UpdateCategoryCommand {Id = id, Title = request.Title, Description = request.Description};

        var result = await _mediator.Send(command);

        return Ok(result);
    }
    
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<CategoryResponse>> DeleteCategory(Guid id)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand {Id = id});

        return Ok(result);
    }
}