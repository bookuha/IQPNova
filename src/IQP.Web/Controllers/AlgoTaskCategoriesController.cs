using IQP.Application.Contracts.AlgoTaskCategories.Commands;
using IQP.Application.Contracts.AlgoTaskCategories.Responses;
using IQP.Application.Services;
using IQP.Web.ViewModels.AlgoTaskCategories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IQP.Web.Controllers;

[Route("api/algo-categories")]
public class AlgoTaskCategoriesController : ControllerBase
{
    private readonly IAlgoTaskCategoriesService _algoTaskCategoriesService;

    public AlgoTaskCategoriesController(IAlgoTaskCategoriesService algoTaskCategoriesService)
    {
        _algoTaskCategoriesService = algoTaskCategoriesService;
    }
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AlgoTaskCategoryResponse>> CreateCategory([FromBody] CreateAlgoTaskCategoryRequest request)
    {
        var command = new CreateAlgoTaskCategoryCommand {Title = request.Title, Description = request.Description};

        var response = await _algoTaskCategoriesService.CreateCategory(command);

        return Created($"api/algo-categories/{response.Id}", response);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlgoTaskCategoryResponse>>> GetCategories()
    {
        var response = await _algoTaskCategoriesService.GetCategories();

        return Ok(response);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<AlgoTaskCategoryResponse>> GetCategoryById(Guid id)
    {
        var response = await _algoTaskCategoriesService.GetCategoryById(id);

        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<AlgoTaskCategoryResponse>> UpdateCategory(Guid id, [FromBody] UpdateAlgoTaskCategoryRequest request)
    {
        var command = new UpdateAlgoTaskCategoryCommand {Id = id, Title = request.Title, Description = request.Description};

        var response = await _algoTaskCategoriesService.UpdateCategory(command);

        return Ok(response);
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(Guid id)
    {
        await _algoTaskCategoriesService.DeleteCategory(id);

        return NoContent();
    }
}