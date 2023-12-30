using IQP.Application.Contracts.Categories.Commands;
using IQP.Application.Contracts.Categories.Responses;
using IQP.Application.Services;
using IQP.Web.ViewModels;
using IQP.Web.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IQP.Web.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly CategoriesService _categoriesService;

    public CategoriesController(CategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var command = new CreateCategoryCommand {Title = request.Title, Description = request.Description};

        var response = await _categoriesService.CreateCategory(command);

        return Created($"api/categories/{response.Id}", response);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponse>> GetCategoryById(Guid id)
    {
        var result = await _categoriesService.GetCategoryById(id);

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories()
    {
        var result = await _categoriesService.GetCategories();

        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryResponse>> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var command = new UpdateCategoryCommand {Id = id, Title = request.Title, Description = request.Description};

        var result = await _categoriesService.UpdateCategory(command);

        return Ok(result);
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<CategoryResponse>> DeleteCategory(Guid id)
    {
        var result = await _categoriesService.DeleteCategory(id);

        return Ok(result);
    }
}