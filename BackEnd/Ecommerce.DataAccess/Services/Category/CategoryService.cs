using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.Entities.DTO.Category;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.DataAccess.Services.Category;

public class CategoryService(AuthContext context, 
    ILogger<CategoryService> logger,
    ResponseHandler responseHandler) : ICategoryService

{
    private readonly AuthContext _context = context;
    private readonly ILogger<CategoryService> _logger = logger;
    private readonly ResponseHandler _responseHandler = responseHandler;

    public async Task<Response<Guid>> AddCategoryAsync(CreateCategoryRequest dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var isExist = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() , cancellationToken);
            if (isExist)
            {
                _logger.LogWarning("Category with name {Name} already exists.", dto.Name);
                return _responseHandler.BadRequest<Guid>("Category with the same name already exists.");
            }

            var category = new Entities.Models.Category
            {
                Name = dto.Name,
                Description = dto.Description
            };
            await _context.Categories.AddAsync(category, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            _logger.LogInformation("Category {CategoryId} added successfully.", category.Id);
            return   responseHandler.Created(category.Id, "Category added successfully.");
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError("Failed to add category.");
            return _responseHandler.InternalServerError<Guid>("Failed to add category.");
        }

            
    }

}