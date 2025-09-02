using System.Linq.Expressions;
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
    
    
    public async Task<Response<List<GetCategoryResponse>>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _context.Categories
            .Where(c => !c.IsDeleted)
            .Select(c => new GetCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return _responseHandler.Success(categories, "Categories retrieved successfully.");
    }

    public async Task<Response<GetCategoryResponse>> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {

        var category = await GetCategoryAsync(c => c.Id == id);
        if (category is null)
        {
            _logger.LogWarning("Category with ID {Id} not found.", id);
            return _responseHandler.NotFound<GetCategoryResponse>("Category not found.");
        }

        return _responseHandler.Success(category, "Category retrieved successfully.");
    }

    public async Task<Response<GetCategoryResponse>> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            _logger.LogWarning("GetCategoryByNameAsync called with empty name.");
            return _responseHandler.BadRequest<GetCategoryResponse>("Invalid category name.");
        }

        var category = await GetCategoryAsync(c => c.Name.ToLower() == name.ToLower());
        if (category is null)
        {
            _logger.LogWarning("Category with name {Name} not found.", name);
            return _responseHandler.NotFound<GetCategoryResponse>("Category not found.");
        }

        return _responseHandler.Success(category, "Category retrieved successfully.");
    }


    public async Task<Response<Guid>> UpdateCategoryAsync(Guid id, UpdateCategoryRequest dto, CancellationToken cancellationToken = default)
    {

        var category = await _context.Categories.FindAsync(id, cancellationToken);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("UpdateCategoryAsync - Category not found. ID: {Id}", id);
            return _responseHandler.NotFound<Guid>("Category not found.");
        }
            
        if (category.Name == dto.Name && category.Description == dto.Description)
        {
            return _responseHandler.BadRequest<Guid>("No changes detected.");
        }

        // Check for duplication with another category (excluding current one)
        var existingCategory = await _context.Categories
            .FirstOrDefaultAsync(c =>
                c.Id != id &&
                c.Name == dto.Name &&
                c.Description == dto.Description &&
                !c.IsDeleted, cancellationToken);

        if (existingCategory != null)
        {
            return _responseHandler.BadRequest<Guid>("Another category with the same name and description already exists.");
        }
        category.Name = dto.Name;
        category.Description = dto.Description;
        category.UpdatedAt = DateTime.UtcNow;
            
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category with ID {Id} updated successfully.", id);

        return _responseHandler.Success(category.Id, "Category updated successfully.");
    }
    
    public async Task<Response<bool>> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {

        var category = await _context.Categories.FindAsync(id, cancellationToken);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("DeleteCategoryAsync - Category not found or already deleted. ID: {Id}", id);
            return _responseHandler.NotFound<bool>("Category not found or already deleted.");
        }

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;

        _logger.LogWarning("Category with ID {Id} is being soft deleted.", id);

        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category with ID {Id} was deleted successfully.", id);

        return _responseHandler.Success(true, "Category deleted successfully.");
    }
    
    
    
    private async Task<GetCategoryResponse?> GetCategoryAsync(Expression<Func<Ecommerce.Entities.Models.Category, bool>> predicate)
    {
        return await _context.Categories
            .Where(predicate)
            .Where(c => !c.IsDeleted)
            .Select(c => new GetCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

}