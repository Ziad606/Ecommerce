using Ecommerce.Entities.DTO.Category;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.Category;

public interface ICategoryService
{
    Task<Response<Guid>> AddCategoryAsync(CreateCategoryRequest dto, CancellationToken cancellationToken = default);
    
}