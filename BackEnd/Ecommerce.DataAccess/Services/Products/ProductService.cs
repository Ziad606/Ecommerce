using System.Linq.Expressions;
using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.ImageUploading;
using Ecommerce.Entities.DTO.Product;
using Ecommerce.Entities.DTO.Products;
using Ecommerce.Entities.DTO.Shared.Product;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Shared;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.DataAccess.Services.Products;

public class ProductService(AuthContext context ,
    ResponseHandler responseHandler, 
    ILogger<ProductService> logger,
    IImageUploadService imageUploadService) : IProductService
{
    private readonly AuthContext _context = context;
    private readonly ResponseHandler _responseHandler = responseHandler;
    private readonly ILogger<ProductService> _logger = logger;
    private readonly IImageUploadService _imageUploadService = imageUploadService;

    public async Task<Response<Guid>> AddProductAsync(CreateProductRequest dto, CancellationToken cancellationToken)
    {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.CategoryId);
            if (category == null || category.IsDeleted)
            {
                _logger.LogWarning(
                    "Category with ID {CategoryId} is either not found or deleted.",
                    dto.CategoryId);
                return responseHandler.BadRequest<Guid>("Invalid category selected.");
            }

            var existingProduct = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p =>
                    p.Name.ToLower().Trim() == dto.Name.ToLower().Trim() &&
                    p.Description.ToLower().Trim() == dto.Description.ToLower().Trim() &&
                    p.Price == dto.Price &&
                    p.CategoryId == dto.CategoryId &&
                    !p.IsDeleted);


            if (existingProduct != null)
            {
                existingProduct.StockQuantity += dto.StockQuantity;
                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Existing product {ProductId} stock increased by admin", existingProduct.Id);

                return responseHandler.Success<Guid>(existingProduct.Id,
                    "Product already exists. Stock quantity has been updated.");
            }

            try
            {
                var productId = Guid.NewGuid();
                var product = new Product
                {
                    Id = productId,
                    Name = dto.Name?.Trim(),
                    Description = dto.Description?.Trim(),
                    Price = dto.Price,
                    CategoryId = dto.CategoryId,
                    Dimensions = dto.Dimensions?.Trim(),
                    Material = dto.Material?.Trim(),
                    SKU = dto.SKU?.Trim(),
                    StockQuantity = dto.StockQuantity,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false,
                    Images = new List<ProductImage>()
                };

                var images = await UploadImagesAsync(dto.Images, productId);
                product.Images = images.ToList();

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Product {ProductId} created successfully by admin", productId);

                return responseHandler.Created(productId,
                    "Product created successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating product for admin");
                return responseHandler.InternalServerError<Guid>(
                    "Database error occurred while creating the product.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating product for admin");
                return responseHandler.InternalServerError<Guid>(
                    "An unexpected error occurred while creating the product.");
            }   
    }
    
    public async Task<Response<PaginatedList<GetProductResponse>>> GetProductsAsync( Expression<Func<Product, bool>> predicate , ProductFilters<ProductSorting> filters,CancellationToken cancellationToken)
    {
        var source =  _context.Products
            .Where(predicate)
            .Include(p => p.Images)
            .Include(p => p.Category)
            .AsQueryable();
        
        var filteredList = FilteredListItems(source, filters)
            .Select(p => new GetProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                Dimensions = p.Dimensions ?? string.Empty,
                Material = p.Material ?? string.Empty,
                SKU = p.SKU ?? string.Empty,
                StockQuantity = p.StockQuantity,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                ImageUrls = p.Images.Select(img => img.ImageUrl).ToList()
            });
            
        var result = await PaginatedList<GetProductResponse>.CreateAsync(filteredList, filters.PageNumber, filters.PageSize, cancellationToken);
        
        

        return _responseHandler.Success(result, "Products retrieved successfully.");
    }
    public async Task<Response<Guid>> UpdateProductAsync(Guid productId, UpdateProductRequest dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("UpdateProductAsync called for ProductId={ProductId}",
                productId);

            try
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

                if (product == null)
                {
                    _logger.LogWarning(
                        "Product update failed: ProductId={ProductId} not found",
                        productId);
                    return _responseHandler.NotFound<Guid>("Product not found");
                }

                if (product.Name != dto.Name?.Trim())
                {
                    _logger.LogInformation("ProductId={ProductId} Name changed from '{Old}' to '{New}'",
                        product.Id, product.Name, dto.Name);
                    product.Name = dto.Name.Trim();
                }

                if (dto.Price.HasValue && dto.Price.Value != product.Price)
                {
                    _logger.LogInformation("ProductId={ProductId} Price changed from {Old} to {New}", product.Id,
                        product.Price, dto.Price.HasValue);
                    product.Price = dto.Price.Value;
                }

                if (dto.StockQuantity.HasValue && dto.StockQuantity.Value != product.StockQuantity)
                {
                    _logger.LogInformation("ProductId={ProductId} StockQuantity changed from {Old} to {New}",
                        product.Id, product.StockQuantity, dto.StockQuantity.Value);

                    product.StockQuantity = dto.StockQuantity.Value;
                }


                if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description != product.Description)
                {
                    _logger.LogInformation("ProductId={ProductId} Description updated", product.Id);
                    product.Description = dto.Description.Trim();
                }

                if (dto.Images != null && dto.Images.Any())
                {
                    var uploadedImages = await ReplaceProductImagesAsync(product.Id, dto.Images);
                    product.Images = uploadedImages.ToList();
                }

                product.UpdatedAt = DateTime.UtcNow;


                await _context.SaveChangesAsync();

                _logger.LogInformation("ProductId={ProductId} updated successfully", product.Id);

                return _responseHandler.Success<Guid>(product.Id, "Product updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating ProductId={ProductId}",
                    productId);
                return _responseHandler.InternalServerError<Guid>("An error occurred while updating the product.");
            }

        }    
    
    public async Task<Response<bool>> DeleteProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p =>
                p.Id == productId && !p.IsDeleted, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Delete failed: Product {ProductId} not found.", productId);
            return _responseHandler.NotFound<bool>("Product not found.");
        }

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} soft-deleted", productId);

        return _responseHandler.Success(true,
            "Product deleted successfully and is no longer visible to buyers.");
    }
    private async Task<IList<ProductImage>> UploadImagesAsync(IEnumerable<IFormFile> files, Guid productId)
    {
        var images = new List<ProductImage>();
        bool isFirstImage = true;

        foreach (var file in files)
        {
            try
            {
                var result = await _imageUploadService.UploadAsync(file);

                if (result == null || string.IsNullOrEmpty(result))
                {
                    _logger.LogError("Failed to upload image {FileName} for product {ProductId}", file.FileName,
                        productId);
                    throw new Exception("Image upload failed.");
                }

                images.Add(new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = result,
                    ProductId = productId,
                    CreatedAt = DateTime.UtcNow,
                    IsPrimary = isFirstImage
                });

                isFirstImage = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while uploading image {FileName} for product {ProductId}",
                    file.FileName, productId);
                throw;
            }
        }

        return images;
    }
    
    private IQueryable<Product> FilteredListItems<TSorting>(IQueryable<Product> query, ProductFilters<TSorting> filters)
        where TSorting : struct, Enum
    {
        
        query = query.Where(p => p.IsActive == filters.Status);
        
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var searchValue = filters.SearchValue.ToLower().Trim();
            query = query.Where(p => 
                p.Name.ToLower().Contains(searchValue) ||
                p.Description.ToLower().Contains(searchValue) ||
                p.SKU.ToLower().Contains(searchValue) ||
                p.Category.Name.ToLower().Contains(searchValue));
            
            _logger.LogInformation("Filtering products by search value: {SearchValue}", filters.SearchValue);
        }

        
        if (filters.SortColumn is not null)
        {
            query = ApplySorting(query, filters.SortColumn.Value, filters.SortDirection);
            _logger.LogInformation("Sorting products by {SortProperty} in {SortDirection} order", filters.SortColumn, filters.SortDirection);
        }
        else
        {
            query = query.OrderByDescending(o => o.CreatedAt);
        }
        
        return query;
    }

    private static IQueryable<Product> ApplySorting<TSorting>(IQueryable<Product> query, TSorting sortColumn, SortDirection? direction)
    {
        var isAscending = direction == SortDirection.ASC;

        return sortColumn switch
        {
            ProductSorting.Name => isAscending ? query.OrderBy(o => o.Name) : query.OrderByDescending(p => p.Name),
            ProductSorting.Price => isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
            ProductSorting.CreatedAt => isAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };
    }
    
    private async Task<IList<ProductImage>> ReplaceProductImagesAsync(Guid productId, IEnumerable<IFormFile> files)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingImages = await _context.ProductImages
                    .Where(i => i.ProductId == productId)
                    .ToListAsync();

                var newImages = new List<ProductImage>();
                bool isFirst = true;

                foreach (var file in files)
                {
                    var uploadResult = await _imageUploadService.UploadAsync(file);

                    if (string.IsNullOrWhiteSpace(uploadResult))
                    {
                        _logger.LogError("Upload failed for image: {FileName}", file.FileName);
                        throw new Exception($"Failed to upload image {file.FileName}");
                    }

                    newImages.Add(new ProductImage
                    {
                        ProductId = productId,
                        ImageUrl = uploadResult,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsPrimary = isFirst
                    });

                    isFirst = false;
                }


                foreach (var oldImage in existingImages)
                {
                    if (!string.IsNullOrWhiteSpace(oldImage.ImageUrl))
                    {
                        try
                        {
                            await _imageUploadService.DeleteAsync(oldImage.ImageUrl);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete image: {ImageUrl}", oldImage);
                        }
                    }
                }

                _context.ProductImages.RemoveRange(existingImages);
                await _context.ProductImages.AddRangeAsync(newImages);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return newImages;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    
}