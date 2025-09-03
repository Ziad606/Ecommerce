using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.Entities.DTO.Wishlist;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Services.Wishlist
{
	public class WishlistService(AuthContext context, ResponseHandler responseHandler, ILogger<WishlistService> logger) : IWishlistService
	{
		private readonly AuthContext _context = context;
		private readonly ResponseHandler _responseHandler = responseHandler;
		private readonly ILogger<WishlistService> _logger = logger;

		public async Task<Response<AddWishlistItemResponse>> AddItemToWishlistAsync(
			AddWishlistItemReq dto, string buyerId, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Adding item to wishlist for BuyerId={BuyerId}, ProductId={ProductId}",
				buyerId, dto.ProductId);

			try
			{
				var product = await _context.Products
					.Include(p => p.Images)
					.FirstOrDefaultAsync(p => p.Id == dto.ProductId && !p.IsDeleted, cancellationToken);

				if (product == null)
				{
					_logger.LogWarning("Product with ID {ProductId} not found or deleted", dto.ProductId);
					return _responseHandler.NotFound<AddWishlistItemResponse>("Product not found.");
				}

				if (!product.IsActive)
				{
					_logger.LogWarning("Product {ProductId} is not active", dto.ProductId);
					return _responseHandler.BadRequest<AddWishlistItemResponse>("Product is not available.");
				}

				var wishlist = await _context.Wishlists
					.Include(w => w.WishlistItems)
					.FirstOrDefaultAsync(w => w.BuyerId == buyerId, cancellationToken);

				if (wishlist == null)
				{
					wishlist = new Ecommerce.Entities.Models.Wishlist
					{
						Id = Guid.NewGuid(),
						BuyerId = buyerId,
						CreatedAt = DateTime.UtcNow,
						WishlistItems = new List<WishlistItem>()
					};

					await _context.Wishlists.AddAsync(wishlist, cancellationToken);
					_logger.LogInformation("New wishlist created for BuyerId={BuyerId}", buyerId);
				}

				var existingWishlistItem = wishlist.WishlistItems
					.FirstOrDefault(wi => wi.ProductId == dto.ProductId);

				if (existingWishlistItem != null)
				{
					_logger.LogWarning("Product {ProductId} already exists in wishlist for BuyerId={BuyerId}",
						dto.ProductId, buyerId);
					return _responseHandler.BadRequest<AddWishlistItemResponse>("Product is already in your wishlist.");
				}

				var newWishlistItem = new WishlistItem
				{
					Id = Guid.NewGuid(),
					WishlistId = wishlist.Id,
					ProductId = dto.ProductId,
					CreatedAt = DateTime.UtcNow
				};

				

				await _context.WishlistItems.AddAsync(newWishlistItem, cancellationToken);
				wishlist.UpdatedAt = DateTime.UtcNow;

				await _context.SaveChangesAsync(cancellationToken);

				var response = new AddWishlistItemResponse
				{
					Id = newWishlistItem.Id,
					ProductId = newWishlistItem.ProductId,
					ProductName = product.Name,
					AddedAt = newWishlistItem.CreatedAt
				};

				_logger.LogInformation("Item successfully added to wishlist for BuyerId={BuyerId}", buyerId);
				return _responseHandler.Success(response, "Item added to wishlist successfully.");
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Database error while adding item to wishlist for BuyerId={BuyerId}", buyerId);
				return _responseHandler.InternalServerError<AddWishlistItemResponse>(
					"Database error occurred while adding item to wishlist.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error while adding item to wishlist for BuyerId={BuyerId}", buyerId);
				return _responseHandler.InternalServerError<AddWishlistItemResponse>(
					"An unexpected error occurred while adding item to wishlist.");
			}
		}



		public async Task<Response<GetWishlistResponse>> GetWishlistAsync(string buyerId, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Retrieving wishlist for BuyerId={BuyerId}", buyerId);

			try
			{
				var wishlist = await _context.Wishlists
					.Include(w => w.WishlistItems)
					.ThenInclude(wi => wi.Product)
					.ThenInclude(p => p.Images)
					.FirstOrDefaultAsync(w => w.BuyerId == buyerId, cancellationToken);

				if (wishlist == null || !wishlist.WishlistItems.Any())
				{
					_logger.LogInformation("No wishlist found or wishlist is empty for BuyerId={BuyerId}", buyerId);

					var emptyWishlist = new GetWishlistResponse
					{
						Id = Guid.Empty,
						Items = new List<WishlistItemDetailsDto>(),
						TotalItems = 0,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = null
					};
					return _responseHandler.Success(emptyWishlist, "Your wishlist is empty.");
				}

				var wishlistItemsDto = wishlist.WishlistItems
					.Where(wi => wi.Product != null && !wi.Product.IsDeleted)
					.Select(wi => new WishlistItemDetailsDto
					{
						Id = wi.Id,
						ProductId = wi.ProductId,
						ProductName = wi.Product.Name,
						ProductPrice = wi.Product.Price,
						ProductImage = wi.Product.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageUrl
									 ?? wi.Product.Images?.FirstOrDefault()?.ImageUrl,
						StockStatus = wi.Product.StockStatus.ToString(),
						IsActive = wi.Product.IsActive,
						AddedAt = wi.CreatedAt
					}).ToList();

				var response = new GetWishlistResponse
				{
					Id = wishlist.Id,
					Items = wishlistItemsDto,
					TotalItems = wishlistItemsDto.Count,
					CreatedAt = wishlist.CreatedAt,
					UpdatedAt = wishlist.UpdatedAt
				};

				_logger.LogInformation("Wishlist retrieved successfully for BuyerId={BuyerId} with {ItemCount} items",
					buyerId, wishlistItemsDto.Count);

				return _responseHandler.Success(response, "Wishlist retrieved successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving wishlist for BuyerId={BuyerId}", buyerId);
				return _responseHandler.InternalServerError<GetWishlistResponse>(
					"An error occurred while retrieving your wishlist.");
			}
		}

		public async Task<Response<bool>> RemoveItemFromWishlistAsync(
			string buyerId, Guid wishlistItemId, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Removing item from wishlist for BuyerId={BuyerId}, WishlistItemId={WishlistItemId}",
				buyerId, wishlistItemId);

			try
			{
				var wishlistItem = await _context.WishlistItems
					.Include(wi => wi.Wishlist)
					.FirstOrDefaultAsync(wi => wi.Id == wishlistItemId &&
											 wi.Wishlist.BuyerId == buyerId, cancellationToken);

				if (wishlistItem == null)
				{
					_logger.LogWarning("Wishlist item {WishlistItemId} not found for BuyerId={BuyerId}",
						wishlistItemId, buyerId);
					return _responseHandler.NotFound<bool>("Wishlist item not found.");
				}

				_context.WishlistItems.Remove(wishlistItem);
				wishlistItem.Wishlist.UpdatedAt = DateTime.UtcNow;

				await _context.SaveChangesAsync(cancellationToken);

				_logger.LogInformation("Wishlist item {WishlistItemId} removed successfully for BuyerId={BuyerId}",
					wishlistItemId, buyerId);

				return _responseHandler.Success(true, "Item removed from wishlist successfully.");
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Database error while removing wishlist item {WishlistItemId} for BuyerId={BuyerId}",
					wishlistItemId, buyerId);
				return _responseHandler.InternalServerError<bool>(
					"Database error occurred while removing item from wishlist.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error while removing wishlist item {WishlistItemId} for BuyerId={BuyerId}",
					wishlistItemId, buyerId);
				return _responseHandler.InternalServerError<bool>(
					"An unexpected error occurred while removing item from wishlist.");
			}
		}

		public async Task<Response<MoveToCartResponse>> MoveItemToCartAsync(
		   string buyerId, Guid wishlistItemId, int quantity, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Moving item from wishlist to cart for BuyerId={BuyerId}, WishlistItemId={WishlistItemId}, Quantity={Quantity}",
				buyerId, wishlistItemId, quantity);

			try
			{
				var wishlistItem = await _context.WishlistItems
					.Include(wi => wi.Wishlist)
					.Include(wi => wi.Product)
					.FirstOrDefaultAsync(wi => wi.Id == wishlistItemId &&
											 wi.Wishlist.BuyerId == buyerId, cancellationToken);

				if (wishlistItem == null)
				{
					_logger.LogWarning("Wishlist item {WishlistItemId} not found for BuyerId={BuyerId}",
						wishlistItemId, buyerId);
					return _responseHandler.NotFound<MoveToCartResponse>("Wishlist item not found.");
				}

				if (wishlistItem.Product == null || wishlistItem.Product.IsDeleted || !wishlistItem.Product.IsActive)
				{
					_logger.LogWarning("Product {ProductId} is not available for wishlist item {WishlistItemId}",
						wishlistItem.ProductId, wishlistItemId);
					return _responseHandler.BadRequest<MoveToCartResponse>("Product is no longer available.");
				}

				if (wishlistItem.Product.StockQuantity < quantity)
				{
					_logger.LogWarning("Insufficient stock for ProductId={ProductId}. Available: {Available}, Requested: {Requested}",
						wishlistItem.ProductId, wishlistItem.Product.StockQuantity, quantity);
					return _responseHandler.BadRequest<MoveToCartResponse>(
						$"Insufficient stock. Only {wishlistItem.Product.StockQuantity} items available.");
				}

				// Get or create cart
				var cart = await _context.Carts
					.Include(c => c.CartItems)
					.FirstOrDefaultAsync(c => c.BuyerId == buyerId, cancellationToken);

				if (cart == null)
				{
					cart = new Cart
					{
						Id = Guid.NewGuid(),
						BuyerId = buyerId,
						CreatedAt = DateTime.UtcNow,
						CartItems = new List<CartItem>()
					};
					await _context.Carts.AddAsync(cart, cancellationToken);
					_logger.LogInformation("New cart created for BuyerId={BuyerId} during wishlist move", buyerId);
				}

				// Check if product already exists in cart
				var existingCartItem = cart.CartItems
					.FirstOrDefault(ci => ci.ProductId == wishlistItem.ProductId);

				if (existingCartItem != null)
				{
					var newTotalQuantity = existingCartItem.Quantity + quantity;
					if (newTotalQuantity > wishlistItem.Product.StockQuantity)
					{
						_logger.LogWarning("Total quantity {TotalQuantity} exceeds stock {Stock} for ProductId={ProductId}",
							newTotalQuantity, wishlistItem.Product.StockQuantity, wishlistItem.ProductId);
						return _responseHandler.BadRequest<MoveToCartResponse>(
							$"Cannot add {quantity} items. Maximum available considering cart: {wishlistItem.Product.StockQuantity - existingCartItem.Quantity}");
					}

					existingCartItem.Quantity = newTotalQuantity;
					existingCartItem.UpdatedAt = DateTime.UtcNow;
				}
				else
				{
					var newCartItem = new CartItem
					{
						Id = Guid.NewGuid(),
						CartId = cart.Id,
						ProductId = wishlistItem.ProductId,
						Quantity = quantity,
						CreatedAt = DateTime.UtcNow
					};
					cart.CartItems.Add(newCartItem);
				}

				cart.UpdatedAt = DateTime.UtcNow;

				// Remove item from wishlist
				_context.WishlistItems.Remove(wishlistItem);
				wishlistItem.Wishlist.UpdatedAt = DateTime.UtcNow;

				await _context.SaveChangesAsync(cancellationToken);

				var response = new MoveToCartResponse
				{
					Success = true,
					Message = "Item moved to cart successfully"
				};

				_logger.LogInformation("Item successfully moved from wishlist to cart for BuyerId={BuyerId}", buyerId);
				return _responseHandler.Success(response, "Item moved to cart successfully.");
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Database error while moving item from wishlist to cart for BuyerId={BuyerId}", buyerId);
				return _responseHandler.InternalServerError<MoveToCartResponse>(
					"Database error occurred while moving item to cart.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error while moving item from wishlist to cart for BuyerId={BuyerId}", buyerId);
				return _responseHandler.InternalServerError<MoveToCartResponse>(
					"An unexpected error occurred while moving item to cart.");
			}
		}

	}
}
