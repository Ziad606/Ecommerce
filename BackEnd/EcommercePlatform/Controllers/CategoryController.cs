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
    
}