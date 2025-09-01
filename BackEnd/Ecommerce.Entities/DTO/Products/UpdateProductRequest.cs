using Microsoft.AspNetCore.Http;

namespace Ecommerce.Entities.DTO.Product;

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public int ?StockQuantity { get; set; }
    public string? Description { get; set; }
    public List<IFormFile>? Images { get; set; }
}