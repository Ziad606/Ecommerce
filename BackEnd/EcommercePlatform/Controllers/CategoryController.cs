using Ecommerce.DataAccess.Services.Category;
using Ecommerce.Entities.DTO.Category;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(ResponseHandler responseHandler, ICategoryService categoryService) : ControllerBase
{
    private readonly ResponseHandler _responseHandler = responseHandler;
    private readonly ICategoryService _categoryService = categoryService;

    [HttpPost("")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddCategory([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));
        }
        
        var result = await _categoryService.AddCategoryAsync(request, cancellationToken);
            
        return StatusCode((int)result.StatusCode, result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllCategoriesAsync();
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _categoryService.GetCategoryByIdAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("by-name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var result = await _categoryService.GetCategoryByNameAsync(name);
        return StatusCode((int)result.StatusCode, result);
    }


    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

        var result = await _categoryService.UpdateCategoryAsync(id, dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }
    
}