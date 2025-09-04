namespace Ecommerce.DataAccess.Services.Review;

using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.Entities.DTO.Reviews;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Models.Reviews;
using Ecommerce.Entities.Shared;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

public class ReviewService : IReviewService
{
    private readonly AuthContext _context;
    private readonly ResponseHandler _responseHandler;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(AuthContext context, ResponseHandler responseHandler, ILogger<ReviewService> logger)
    {
        _context = context;
        _responseHandler = responseHandler;
        _logger = logger;
    }

    public async Task<Response<Guid>> CreateReviewAsync(CreateReviewRequest dto, string buyerId)
    {
        if (dto == null)
        {
            _logger.LogWarning("Review data was null.");
            return _responseHandler.BadRequest<Guid>("Review data is required.");
        }

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == dto.OrderId && !o.IsDeleted);
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found.", dto.OrderId);
            return _responseHandler.NotFound<Guid>("Order not found.");
        }
        if (order.BuyerId != buyerId)
        {
            _logger.LogWarning("Buyer {BuyerId} attempted to review non-owned order {OrderId}.", buyerId, dto.OrderId);
            return _responseHandler.BadRequest<Guid>("You can only review your own orders.");
        }
        if (order.Status != OrderStatus.Delivered)
        {
            _logger.LogWarning("Order {OrderId} is not delivered.", dto.OrderId);
            return _responseHandler.BadRequest<Guid>("Reviews are only allowed for delivered orders.");
        }
        if (!order.OrderItems.Any(oi => oi.ProductId == dto.ProductId))
        {
            _logger.LogWarning("Product {ProductId} not in order {OrderId}.", dto.ProductId, dto.OrderId);
            return _responseHandler.BadRequest<Guid>("Product not part of this order.");
        }

        var existingReview = await _context.Reviews
            .FirstOrDefaultAsync(r => r.OrderId == dto.OrderId && r.ProductId == dto.ProductId && r.BuyerId == buyerId && !r.IsDeleted);
        if (existingReview != null)
        {
            _logger.LogWarning("Duplicate review for order {OrderId}, product {ProductId}.", dto.OrderId, dto.ProductId);
            return _responseHandler.BadRequest<Guid>("You have already reviewed this product for this order.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var review = new Review
            {
                Id = Guid.NewGuid(),
                OrderId = dto.OrderId,
                ProductId = dto.ProductId,
                BuyerId = buyerId,
                Rating = (double)dto.Rating, // Map enum to double
                Comment = dto.Comment?.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.Reviews.AddAsync(review);
            await UpdateProductAverageRatingAsync(dto.ProductId);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine($"Notification: Review {review.Id} created for product {dto.ProductId} by buyer {buyerId}");
            _logger.LogInformation("Review {ReviewId} created successfully.", review.Id);
            return _responseHandler.Created(review.Id, "Review created successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating review.");
            return _responseHandler.InternalServerError<Guid>("Failed to create review: " + ex.Message);
        }
    }

    public async Task<Response<GetReviewsResponse>> UpdateReviewAsync(Guid id, UpdateReviewRequest dto, string buyerId)
    {
        if (dto == null || !dto.Confirm)
        {
            _logger.LogWarning("Update not confirmed for review {ReviewId}.", id);
            return _responseHandler.BadRequest<GetReviewsResponse>("Update not confirmed.");
        }

        var review = await _context.Reviews
            .Include(r => r.Buyer)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (review == null)
        {
            _logger.LogWarning("Review {ReviewId} not found.", id);
            return _responseHandler.NotFound<GetReviewsResponse>("Review not found.");
        }
        if (review.BuyerId != buyerId)
        {
            _logger.LogWarning("Buyer {BuyerId} attempted to update non-owned review {ReviewId}.", buyerId, id);
            return _responseHandler.BadRequest<GetReviewsResponse>("You can only update your own reviews.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            review.Rating = (double)dto.Rating; // Map enum to double
            review.Comment = dto.Comment?.Trim();
            review.UpdatedAt = DateTime.UtcNow;

            await UpdateProductAverageRatingAsync(review.ProductId);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine($"Notification: Review {id} updated by buyer {buyerId}");
            _logger.LogInformation("Review {ReviewId} updated successfully.", id);
            return _responseHandler.Success(new GetReviewsResponse
            {
                Id = review.Id,
                OrderId = review.OrderId,
                ProductId = review.ProductId,
                BuyerName = (review.Buyer?.FirstName ?? "") + " " + (review.Buyer?.LastName ?? ""),
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            }, "Review updated successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating review {ReviewId}.", id);
            return _responseHandler.InternalServerError<GetReviewsResponse>("Failed to update review: " + ex.Message);
        }
    }

    public async Task<Response<string>> DeleteReviewAsync(Guid id, DeleteReviewRequest dto, string buyerId)
    {
        if (!dto.Confirm)
        {
            _logger.LogWarning("Deletion not confirmed for review {ReviewId}.", id);
            return _responseHandler.BadRequest<string>("Deletion not confirmed.");
        }

        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (review == null)
        {
            _logger.LogWarning("Review {ReviewId} not found.", id);
            return _responseHandler.NotFound<string>("Review not found.");
        }
        if (review.BuyerId != buyerId)
        {
            _logger.LogWarning("Buyer {BuyerId} attempted to delete non-owned review {ReviewId}.", buyerId, id);
            return _responseHandler.BadRequest<string>("You can only delete your own reviews.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            review.IsDeleted = true;
            await UpdateProductAverageRatingAsync(review.ProductId);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine($"Notification: Review {id} deleted by buyer {buyerId}");
            _logger.LogInformation("Review {ReviewId} deleted successfully.", id);
            return _responseHandler.Deleted<string>("Review deleted successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error deleting review {ReviewId}.", id);
            return _responseHandler.InternalServerError<string>("Failed to delete review: " + ex.Message);
        }
    }

    public async Task<Response<PaginatedList<GetReviewsResponse>>> GetAllReviewsAsync(GetReviewsRequest dto)
    {
        try
        {
            var query = _context.Reviews
                .Where(r => !r.IsDeleted)
                .Include(r => r.Buyer)
                .Include(r => r.Product)
                .AsQueryable();

            if (dto.OrderId != Guid.Empty)
                query = query.Where(r => r.OrderId == dto.OrderId);
            else if (dto.ProductId != Guid.Empty)
                query = query.Where(r => r.ProductId == dto.ProductId);
            else
                return _responseHandler.BadRequest<PaginatedList<GetReviewsResponse>>("OrderId or ProductId required.");

            if (!string.IsNullOrEmpty(dto.SearchValue))
            {
                var searchValue = dto.SearchValue.ToLower().Trim();
                query = query.Where(r => r.Comment.ToLower().Contains(searchValue) ||
                                         (r.Buyer.FirstName ?? "").ToLower().Contains(searchValue) ||
                                         (r.Buyer.LastName ?? "").ToLower().Contains(searchValue));
                _logger.LogInformation("Filtering reviews by {SearchValue}.", dto.SearchValue);
            }

            if (dto.SortColumn.HasValue)
            {
                query = ApplySorting(query, dto.SortColumn.Value, dto.SortDirection);
                _logger.LogInformation("Sorting reviews by {SortColumn} {SortDirection}.", dto.SortColumn, dto.SortDirection);
            }
            else
            {
                query = query.OrderByDescending(r => r.CreatedAt);
            }

            var paginated = await PaginatedList<GetReviewsResponse>.CreateAsync(
                query.Select(r => new GetReviewsResponse
                {
                    Id = r.Id,
                    OrderId = r.OrderId,
                    ProductId = r.ProductId,
                    BuyerName = r.Buyer.FirstName + " " + r.Buyer.LastName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                }), dto.PageNumber, dto.PageSize);

            return _responseHandler.Success(paginated, "Reviews retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reviews.");
            return _responseHandler.InternalServerError<PaginatedList<GetReviewsResponse>>("Failed to retrieve reviews: " + ex.Message);
        }
    }

    public async Task<Response<GetReviewsResponse>> GetReviewByIdAsync(Guid id)
    {
        try
        {
            var review = await _context.Reviews
                .Where(r => r.Id == id && !r.IsDeleted)
                .Include(r => r.Buyer)
                .Include(r => r.Product)
                .Select(r => new GetReviewsResponse
                {
                    Id = r.Id,
                    OrderId = r.OrderId,
                    ProductId = r.ProductId,
                    BuyerName = r.Buyer.FirstName + " " + r.Buyer.LastName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (review == null)
            {
                _logger.LogWarning("Review {ReviewId} not found.", id);
                return _responseHandler.NotFound<GetReviewsResponse>("Review not found.");
            }

            return _responseHandler.Success(review, "Review retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving review {ReviewId}.", id);
            return _responseHandler.InternalServerError<GetReviewsResponse>("Failed to retrieve review: " + ex.Message);
        }
    }

    private async Task UpdateProductAverageRatingAsync(Guid productId)
    {
        var product = await _context.Products
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
        if (product != null)
        {
            var activeReviews = product.Reviews.Where(r => !r.IsDeleted).ToList();
            product.AverageRating = activeReviews.Any() ? activeReviews.Average(r => r.Rating) : 0;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }

    private static IQueryable<Review> ApplySorting(IQueryable<Review> query, ReviewSorting sortColumn, SortDirection? direction)
    {
        var isAscending = direction == SortDirection.ASC;
        return sortColumn switch
        {
            ReviewSorting.Rating => isAscending ? query.OrderBy(r => r.Rating) : query.OrderByDescending(r => r.Rating),
            ReviewSorting.CreatedAt => isAscending ? query.OrderBy(r => r.CreatedAt) : query.OrderByDescending(r => r.CreatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };
    }
}