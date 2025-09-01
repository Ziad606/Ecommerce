using System.Linq.Expressions;
using Ecommerce.Entities.DTO.Product;
using Ecommerce.Entities.DTO.Products;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.Products;

public interface IProductService
{
        Task<Response<Guid>> AddProductAsync(CreateProductRequest dto, CancellationToken cancellationToken = default);
        Task<Response<List<GetProductResponse>>> GetProductsAsync(Expression<Func<Product, bool>> predicate , CancellationToken cancellationToken = default);

            
}