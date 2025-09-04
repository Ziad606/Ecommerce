using Ecommerce.DataAccess.Services.Products;
using Ecommerce.Entities.DTO.Product;
using Ecommerce.Entities.DTO.Products;
using Ecommerce.Entities.DTO.Shared.Product;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductController(IProductService productService, ResponseHandler responseHandler) : ControllerBase
{
    private readonly IProductService _productService = productService;
    private readonly ResponseHandler _responseHandler = responseHandler;

    [HttpPost("")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddProduct([FromForm] CreateProductRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

        var result = await _productService.AddProductAsync(request, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetProducts([FromQuery] ProductFilters<ProductSorting> filters, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

        var result = await _productService.GetProductsAsync(p => !p.IsDeleted, filters, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<Response<GetProductResponse>>> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productService.GetProductByIdAsync(id, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }



    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromForm] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

        var result = await _productService.UpdateProductAsync(id, request, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _productService.DeleteProductAsync(id, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }
}