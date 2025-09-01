using Ecommerce.Entities.DTO.Products;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.Products;

public interface IProductService
{
        Task<Response<Guid>> AddProductAsync(CreateProductRequest dto);
    
}