using Ecommerce.DataAccess.Services.Products;
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
   public async Task<IActionResult> AddProduct([FromForm] CreateProductRequest request)
   {
      if (!ModelState.IsValid)
         return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));
         
      var result = await _productService.AddProductAsync(request);
      return StatusCode((int)result.StatusCode, result);
   }
   
   [HttpGet("")]
   public async Task<IActionResult> GetProducts([FromQuery] ProductFilters<ProductSorting> filters,CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
         return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));
      
      var result = await _productService.GetProductsAsync(p => !p.IsDeleted ,filters ,cancellationToken);
      return StatusCode((int)result.StatusCode, result);
   }
   
   
   
}