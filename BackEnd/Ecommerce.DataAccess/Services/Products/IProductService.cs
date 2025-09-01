using System.Linq.Expressions;
using Ecommerce.Entities.DTO.Product;
using Ecommerce.Entities.DTO.Products;
using Ecommerce.Entities.DTO.Shared.Product;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Shared;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.DataAccess.Services.Products;

public interface IProductService
{
        Task<Response<Guid>> AddProductAsync(CreateProductRequest dto, CancellationToken cancellationToken = default);
        Task<Response<PaginatedList<GetProductResponse>>> GetProductsAsync( Expression<Func<Product, bool>> predicate , ProductFilters<ProductSorting> filters,CancellationToken cancellationToken = default);
        Task<Response<Guid>> UpdateProductAsync(Guid productId, UpdateProductRequest dto,
                CancellationToken cancellationToken = default);

}