using Ecommerce.Entities.DTO.Category;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.Category;

public interface ICategoryService
{
    Task<Response<Guid>> AddCategoryAsync(CreateCategoryRequest dto, CancellationToken cancellationToken = default);
    Task<Response<List<GetCategoryResponse>>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Response<GetCategoryResponse>> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Response<GetCategoryResponse>> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Response<bool>> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Response<Guid>> UpdateCategoryAsync(Guid id, UpdateCategoryRequest dto, CancellationToken cancellationToken = default);
    
}