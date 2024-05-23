using IQP.Application.Contracts.AlgoTaskCategories.Responses;
using IQP.Application.Services;
using IQP.Application.Usecases.AlgoCategories.Create;
using IQP.Application.Usecases.AlgoCategories.Delete;
using IQP.Application.Usecases.AlgoCategories.Get;
using IQP.Application.Usecases.AlgoCategories.GetById;
using IQP.Application.Usecases.AlgoCategories.Update;
using IQP.Web.ViewModels.AlgoTaskCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IQP.Web.Controllers;

[Route("api/algo-categories")]
public class AlgoTaskCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AlgoTaskCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult<AlgoTaskCategoryResponse>> CreateCategory([FromBody] CreateAlgoTaskCategoryRequest request)
    {
        var command = new CreateAlgoTaskCategoryCommand {Title = request.Title, Description = request.Description};

        var response = await _mediator.Send(command);

        return Created($"api/algo-categories/{response.Id}", response);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlgoTaskCategoryResponse>>> GetCategories()
    {
        var response = await _mediator.Send(new GetAlgoTaskCategoriesQuery());

        return Ok(response);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<AlgoTaskCategoryResponse>> GetCategoryById(Guid id)
    {
        var response = await _mediator.Send(new GetAlgoTaskCategoryByIdQuery {Id = id});

        return Ok(response);
    }
    
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<ActionResult<AlgoTaskCategoryResponse>> UpdateCategory(Guid id, [FromBody] UpdateAlgoTaskCategoryRequest request)
    {
        var command = new UpdateAlgoTaskCategoryCommand {Id = id, Title = request.Title, Description = request.Description};

        var response = await _mediator.Send(command);

        return Ok(response);
    }
    
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(Guid id)
    {
        await _mediator.Send(new DeleteAlgoTaskCategoryCommand {Id = id});

        return NoContent();
    }
}