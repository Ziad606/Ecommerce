using Microsoft.AspNetCore.Http;

namespace Ecommerce.Entities.DTO.Products;

public class CreateProductRequest
{
    public string Name { get; set; }
    public Guid CategoryId { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string? Dimensions { get; set; }
    public string? Material { get; set; }
    public string? SKU { get; set; }
    public int StockQuantity { get; set; }
    public List<IFormFile> Images { get; set; }
}