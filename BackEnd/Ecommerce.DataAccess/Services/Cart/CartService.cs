using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.Entities.DTO.Cart;
using Ecommerce.Entities.DTO.CartDTOs;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Implementations
{
	public class CartService(AuthContext context, ResponseHandler responseHandler, ILogger<CartService> logger) : ICartService
	{
		private readonly AuthContext _context = context;
		private readonly ResponseHandler _responseHandler = responseHandler;
		private readonly ILogger<CartService> _logger = logger;




		public async Task<Response<AddCartResponse>> AddItemToCartAsync(AddCartReq dto, string buyerId, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Adding item to cart for BuyerId={BuyerId}, ProductId={ProductId}, Quantity={Quantity}",
				buyerId, dto.ProductId, dto.Quantity);

			try
			{
				var product = await _context.Products
					.Include(p => p.Images)
					.FirstOrDefaultAsync(p => p.Id == dto.ProductId && !p.IsDeleted, cancellationToken);

				if (product == null)
				{
					_logger.LogWarning("Product with ID {ProductId} not found or deleted", dto.ProductId);
					return _responseHandler.NotFound<AddCartResponse>("Product not found.");
				}

				if (!product.IsActive)
				{
					_logger.LogWarning("Product {ProductId} is not active", dto.ProductId);
					return _responseHandler.BadRequest<AddCartResponse>("Product is not available.");
				}

				if (product.StockQuantity < dto.Quantity)
				{
					_logger.LogWarning("Insufficient stock for ProductId={ProductId}. Available: {Available}, Requested: {Requested}",
						dto.ProductId, product.StockQuantity, dto.Quantity);
					return _responseHandler.BadRequest<AddCartResponse>("Insufficient stock available.");
				}

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
					_logger.LogInformation("New cart created for BuyerId={BuyerId}", buyerId);
				}

				var existingCartItem = cart.CartItems
					.FirstOrDefault(ci => ci.ProductId == dto.ProductId);

				CartItem targetCartItem;

				if (existingCartItem != null)
				{
					var newTotalQuantity = existingCartItem.Quantity + dto.Quantity;
					if (newTotalQuantity > product.StockQuantity)
					{
						_logger.LogWarning("Total quantity {TotalQuantity} exceeds stock {Stock} for ProductId={ProductId}",
							newTotalQuantity, product.StockQuantity, dto.ProductId);
						return _responseHandler.BadRequest<AddCartResponse>(
							$"Cannot add {dto.Quantity} items. Maximum available: {product.StockQuantity - existingCartItem.Quantity}");
					}

					existingCartItem.Quantity = newTotalQuantity;
					existingCartItem.UpdatedAt = DateTime.UtcNow;
					targetCartItem = existingCartItem;

					_logger.LogInformation("Updated existing cart item {CartItemId} quantity to {Quantity}",
						existingCartItem.Id, newTotalQuantity);
				}
				else
				{
					var newCartItem = new CartItem
					{
						Id = Guid.NewGuid(),
						CartId = cart.Id,
						ProductId = dto.ProductId,
						Quantity = dto.Quantity,
						CreatedAt = DateTime.UtcNow
					};
					await _context.CartItems.AddAsync(newCartItem, cancellationToken);
					targetCartItem = newCartItem;

					_logger.LogInformation("Added new cart item {CartItemId} for ProductId={ProductId}",
						newCartItem.Id, dto.ProductId);
				}

				cart.UpdatedAt = DateTime.UtcNow;
				await _context.SaveChangesAsync(cancellationToken);

				var response = new AddCartResponse
				{
					Id = targetCartItem.Id,
					ProductId = targetCartItem.ProductId,
					ProductName = product.Name,
					ProductPrice = product.Price,
					Quantity = targetCartItem.Quantity
				};

				_logger.LogInformation("Item successfully added to cart for BuyerId={BuyerId}", buyerId);
				return _responseHandler.Success(response, "Item added to cart successfully.");
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Database error while adding item to cart for BuyerId={BuyerId}", buyerId);
				return _responseHandler.InternalServerError<AddCartResponse>(
					"Database error occurred while adding item to cart.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error while adding item to cart for BuyerId={BuyerId}", buyerId);
				return _responseHandler.InternalServerError<AddCartResponse>(
					"An unexpected error occurred while adding item to cart.");
			}
		}
		//public async Task<Response<AddCartResponse>> AddItemToCartAsync(AddCartReq dto, string buyerId, CancellationToken cancellationToken = default)
		//{
		//	_logger.LogInformation("Adding item to cart for BuyerId={BuyerId}, ProductId={ProductId}, Quantity={Quantity}",
		//		buyerId, dto.ProductId, dto.Quantity);

		//	try
		//	{
		//		var product = await _context.Products
		//			.Include(p => p.Images)
		//			.FirstOrDefaultAsync(p => p.Id == dto.ProductId && !p.IsDeleted, cancellationToken);

		//		if (product == null)
		//		{
		//			_logger.LogWarning("Product with ID {ProductId} not found or deleted", dto.ProductId);
		//			return _responseHandler.NotFound<AddCartResponse>("Product not found.");
		//		}

		//		if (!product.IsActive)
		//		{
		//			_logger.LogWarning("Product {ProductId} is not active", dto.ProductId);
		//			return _responseHandler.BadRequest<AddCartResponse>("Product is not available.");
		//		}

		//		if (product.StockQuantity < dto.Quantity)
		//		{
		//			_logger.LogWarning("Insufficient stock for ProductId={ProductId}. Available: {Available}, Requested: {Requested}",
		//				dto.ProductId, product.StockQuantity, dto.Quantity);
		//			return _responseHandler.BadRequest<AddCartResponse>("Insufficient stock available.");
		//		}

		//		var cart = await _context.Carts
		//			.Include(c => c.CartItems)
		//			.FirstOrDefaultAsync(c => c.BuyerId == buyerId, cancellationToken);

		//		if (cart == null)
		//		{
		//			cart = new Cart
		//			{
		//				Id = Guid.NewGuid(),
		//				BuyerId = buyerId,
		//				CreatedAt = DateTime.UtcNow,
		//				CartItems = new List<CartItem>()
		//			};
		//			await _context.Carts.AddAsync(cart, cancellationToken);
		//			_logger.LogInformation("New cart created for BuyerId={BuyerId}", buyerId);
		//		}

		//		var existingCartItem = cart.CartItems
		//			.FirstOrDefault(ci => ci.ProductId == dto.ProductId);

		//		CartItem targetCartItem;

		//		if (existingCartItem != null)
		//		{
		//			var newTotalQuantity = existingCartItem.Quantity + dto.Quantity;
		//			if (newTotalQuantity > product.StockQuantity)
		//			{
		//				_logger.LogWarning("Total quantity {TotalQuantity} exceeds stock {Stock} for ProductId={ProductId}",
		//					newTotalQuantity, product.StockQuantity, dto.ProductId);
		//				return _responseHandler.BadRequest<AddCartResponse>(
		//					$"Cannot add {dto.Quantity} items. Maximum available: {product.StockQuantity - existingCartItem.Quantity}");
		//			}

		//			existingCartItem.Quantity = newTotalQuantity;
		//			existingCartItem.UpdatedAt = DateTime.UtcNow;
		//			targetCartItem = existingCartItem;

		//			_logger.LogInformation("Updated existing cart item {CartItemId} quantity to {Quantity}",
		//				existingCartItem.Id, newTotalQuantity);
		//		}
		//		else
		//		{
		//			var newCartItem = new CartItem
		//			{
		//				Id = Guid.NewGuid(),
		//				CartId = cart.Id,
		//				ProductId = dto.ProductId,
		//				Quantity = dto.Quantity,
		//				CreatedAt = DateTime.UtcNow
		//			};


		//			await _context.CartItems.AddAsync(newCartItem, cancellationToken);


		//			targetCartItem = newCartItem;

		//			_logger.LogInformation("Added new cart item {CartItemId} for ProductId={ProductId}",
		//				newCartItem.Id, dto.ProductId);
		//		}

		//		cart.UpdatedAt = DateTime.UtcNow;
		//		await _context.SaveChangesAsync(cancellationToken);

		//		var response = new AddCartResponse
		//		{
		//			Id = targetCartItem.Id,
		//			ProductId = targetCartItem.ProductId,
		//			ProductName = product.Name,
		//			ProductPrice = product.Price,
		//			Quantity = targetCartItem.Quantity
		//		};

		//		_logger.LogInformation("Item successfully added to cart for BuyerId={BuyerId}", buyerId);
		//		return _responseHandler.Success(response, "Item added to cart successfully.");
		//	}
		//	catch (DbUpdateException ex)
		//	{
		//		_logger.LogError(ex, "Database error while adding item to cart for BuyerId={BuyerId}", buyerId);
		//		return _responseHandler.InternalServerError<AddCartResponse>(
		//			"Database error occurred while adding item to cart.");
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.LogError(ex, "Unexpected error while adding item to cart for BuyerId={BuyerId}", buyerId);
		//		return _responseHandler.InternalServerError<AddCartResponse>(
		//			"An unexpected error occurred while adding item to cart.");
		//	}
		//}

		public async Task<Response<GetCartResponse>> GetCartAsync(string buyerId, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Retrieving cart for BuyerId={BuyerId}", buyerId);

			try
			{
				var cart = await _context.Carts
					.Include(c => c.CartItems)
					.ThenInclude(ci => ci.Product)
					.ThenInclude(p => p.Images)
					.FirstOrDefaultAsync(c => c.BuyerId == buyerId, cancellationToken);

				if (cart == null || !cart.CartItems.Any())
				{
					_logger.LogInformation("No cart found or cart is empty for BuyerId={BuyerId}", buyerId);

					var emptyCart = new GetCartResponse
					{
						Id = Guid.Empty,
						Items = new List<CartItemDetailsDto>(),
						TotalItems = 0,
						TotalPrice = 0,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = null
					};
					return _responseHandler.Success(emptyCart, "Your cart is empty.");
				}

				var cartItemsDto = cart.CartItems
					.Where(ci => ci.Product != null && !ci.Product.IsDeleted && ci.Product.IsActive)
					.Select(ci => new CartItemDetailsDto
					{
						Id = ci.Id,
						ProductId = ci.ProductId,
						ProductName = ci.Product.Name,
						ProductPrice = ci.Product.Price,
						ProductImage = ci.Product.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageUrl
									 ?? ci.Product.Images?.FirstOrDefault()?.ImageUrl,
						Quantity = ci.Quantity,
						StockStatus = ci.Product.StockStatus.ToString(),
						StockQuantity = ci.Product.StockQuantity
					}).ToList();

				var totalItems = cartItemsDto.Sum(item => item.Quantity);
				var totalPrice = cartItemsDto.Sum(item => item.Subtotal);

				var response = new GetCartResponse
				{
					Id = cart.Id,
					Items = cartItemsDto,
					TotalItems = totalItems,
					TotalPrice = totalPrice,
					CreatedAt = cart.CreatedAt,
					UpdatedAt = cart.UpdatedAt
				};

				_logger.LogInformation("Cart retrieved successfully for BuyerId={BuyerId} with {ItemCount} items",
					buyerId, cartItemsDto.Count);

				return _responseHandler.Success(response, "Cart retrieved successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving cart for BuyerId={BuyerId}", buyerId);
				return _responseHandler.InternalServerError<GetCartResponse>(
					"An error occurred while retrieving your cart.");
			}
		}

		public async Task<Response<UpdateCartResponse>> UpdateCartItemQuantityAsync(
			string buyerId, Guid cartItemId, int quantity, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Updating cart item quantity for BuyerId={BuyerId}, CartItemId={CartItemId}, NewQuantity={Quantity}",
				buyerId, cartItemId, quantity);

			try
			{
				var cartItem = await _context.CartItems
					.Include(ci => ci.Cart)
					.Include(ci => ci.Product)
					.FirstOrDefaultAsync(ci => ci.Id == cartItemId &&
											 ci.Cart.BuyerId == buyerId, cancellationToken);

				if (cartItem == null)
				{
					_logger.LogWarning("Cart item {CartItemId} not found for BuyerId={BuyerId}", cartItemId, buyerId);
					return _responseHandler.NotFound<UpdateCartResponse>("Cart item not found.");
				}

				if (cartItem.Product == null || cartItem.Product.IsDeleted || !cartItem.Product.IsActive)
				{
					_logger.LogWarning("Product {ProductId} is not available for cart item {CartItemId}",
						cartItem.ProductId, cartItemId);
					return _responseHandler.BadRequest<UpdateCartResponse>("Product is no longer available.");
				}

				if (quantity <= 0)
				{
					_logger.LogInformation("Removing cart item {CartItemId} due to zero quantity", cartItemId);

					
					cartItem.UpdatedAt = DateTime.UtcNow;
					cartItem.Cart.UpdatedAt = DateTime.UtcNow;

					await _context.SaveChangesAsync(cancellationToken);

					return _responseHandler.Success(new UpdateCartResponse
					{
						Id = cartItem.Id,
						Quantity = 0,
						Subtotal = 0
					}, "Item removed from cart.");
				}

				if (cartItem.Product.StockQuantity < quantity)
				{
					_logger.LogWarning("Insufficient stock for ProductId={ProductId}. Available: {Available}, Requested: {Requested}",
						cartItem.ProductId, cartItem.Product.StockQuantity, quantity);
					return _responseHandler.BadRequest<UpdateCartResponse>(
						$"Insufficient stock. Only {cartItem.Product.StockQuantity} items available.");
				}

				cartItem.Quantity = quantity;
				cartItem.UpdatedAt = DateTime.UtcNow;
				cartItem.Cart.UpdatedAt = DateTime.UtcNow;

				await _context.SaveChangesAsync(cancellationToken);

				var subtotal = cartItem.Product.Price * cartItem.Quantity;

				var response = new UpdateCartResponse
				{
					Id = cartItem.Id,
					Quantity = cartItem.Quantity,
					Subtotal = subtotal
				};

				_logger.LogInformation("Cart item {CartItemId} quantity updated to {Quantity}", cartItemId, quantity);

				return _responseHandler.Success(response, "Cart item updated successfully.");
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Database error while updating cart item {CartItemId} for BuyerId={BuyerId}",
					cartItemId, buyerId);
				return _responseHandler.InternalServerError<UpdateCartResponse>(
					"Database error occurred while updating cart item.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error while updating cart item {CartItemId} for BuyerId={BuyerId}",
					cartItemId, buyerId);
				return _responseHandler.InternalServerError<UpdateCartResponse>(
					"An unexpected error occurred while updating cart item.");
			}
		}

		public async Task<Response<bool>> RemoveItemFromCartAsync(string buyerId, Guid cartItemId, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Removing item from cart for BuyerId={BuyerId}, CartItemId={CartItemId}",
				buyerId, cartItemId);

			try
			{
				var cartItem = await _context.CartItems
					.Include(ci => ci.Cart)
					.FirstOrDefaultAsync(ci => ci.Id == cartItemId &&
											 ci.Cart.BuyerId == buyerId, cancellationToken);

				if (cartItem == null)
				{
					_logger.LogWarning("Cart item {CartItemId} not found for BuyerId={BuyerId}", cartItemId, buyerId);
					return _responseHandler.NotFound<bool>("Cart item not found.");
				}

				
				cartItem.UpdatedAt = DateTime.UtcNow;
				cartItem.Cart.UpdatedAt = DateTime.UtcNow;

				await _context.SaveChangesAsync(cancellationToken);

				_logger.LogInformation("Cart item {CartItemId} removed successfully for BuyerId={BuyerId}",
					cartItemId, buyerId);

				return _responseHandler.Success(true, "Item removed from cart successfully.");
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Database error while removing cart item {CartItemId} for BuyerId={BuyerId}",
					cartItemId, buyerId);
				return _responseHandler.InternalServerError<bool>(
					"Database error occurred while removing item from cart.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error while removing cart item {CartItemId} for BuyerId={BuyerId}",
					cartItemId, buyerId);
				return _responseHandler.InternalServerError<bool>(
					"An unexpected error occurred while removing item from cart.");
			}
		}

		public async Task<Response<bool>> ClearCartAsync(string buyerId, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Clearing cart for BuyerId={BuyerId}", buyerId);

			try
			{
				var cart = await _context.Carts
					.Include(c => c.CartItems)
					.FirstOrDefaultAsync(c => c.BuyerId == buyerId, cancellationToken);

				if (cart == null)
				{
					_logger.LogInformation("No cart found for BuyerId={BuyerId}", buyerId);
					return _responseHandler.Success(true, "Cart is already empty.");
				}

				foreach (var item in cart.CartItems)
				{
					_context.CartItems.Remove(item);
				}

				cart.UpdatedAt = DateTime.UtcNow;

				await _context.SaveChangesAsync(cancellationToken);

				_logger.LogInformation("Cart cleared successfully for BuyerId={BuyerId}", buyerId);

				return _responseHandler.Success(true, "Cart cleared successfully.");
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Database error while clearing cart for BuyerId={BuyerId}", buyerId);
				return _responseHandler.InternalServerError<bool>(
					"Database error occurred while clearing cart.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error while clearing cart for BuyerId={BuyerId}", buyerId);
				return _responseHandler.InternalServerError<bool>(
					"An unexpected error occurred while clearing cart.");
			}
		}
	}
}