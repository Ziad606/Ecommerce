using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.Entities.DTO.CartDTOs;
using Ecommerce.Entities.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Implementations
{
	public class CartService : ICartService
	{
		private readonly AuthContext _context;

		public CartService(AuthContext context)
		{
			_context = context;
		}

		public async Task<AddCartResponse?> AddItemToCartAsync(AddCartReq dto, string buyerId)
		{
			// تأكد إن الـ Product موجود واجلبه مع بياناته
			var product = await _context.Products
				.Include(p => p.Images)
				.FirstOrDefaultAsync(p => p.Id == dto.ProductId);

			if (product == null)
				return null; // المنتج مش موجود

			if (product.StockQuantity < dto.Quantity)
				return null; // مش كفاية في المخزون

			// دور على الكارت بتاع الـ Buyer
			var cart = await _context.Carts
				.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.ThenInclude(p => p.Images)
				.FirstOrDefaultAsync(c => c.BuyerId == buyerId);

			if (cart == null)
			{
				cart = new Cart
				{
					Id = Guid.NewGuid(),
					BuyerId = buyerId,
					CreatedAt = DateTime.UtcNow,
					CartItems = new List<CartItem>()
				};
				await _context.Carts.AddAsync(cart);
			}

			// شوف لو المنتج موجود بالفعل في الكارت
			var existingCartItem = cart.CartItems
				.FirstOrDefault(ci => ci.ProductId == dto.ProductId);

			CartItem targetCartItem;

			if (existingCartItem != null)
			{
				// تحقق من إن الكمية الجديدة ما تتجاوزش المخزون
				if (existingCartItem.Quantity + dto.Quantity > product.StockQuantity)
					return null;

				existingCartItem.Quantity += dto.Quantity;
				existingCartItem.UpdatedAt = DateTime.UtcNow;
				targetCartItem = existingCartItem;
			}
			else
			{
				var newCartItem = new CartItem
				{
					Id = Guid.NewGuid(),
					CartId = cart.Id,
					ProductId = dto.ProductId,
					Quantity = dto.Quantity,
					CreatedAt = DateTime.UtcNow,
					Product = product // ربط المنتج للحصول على البيانات
				};
				cart.CartItems.Add(newCartItem);
				targetCartItem = newCartItem;
			}

			cart.UpdatedAt = DateTime.UtcNow;

			// حفظ التغييرات
			await _context.SaveChangesAsync();

			// إرجاع الـ Response DTO
			return new AddCartResponse
			{
				Id = targetCartItem.Id,
				ProductId = targetCartItem.ProductId,
				ProductName = product.Name,
				ProductPrice = product.Price,
				Quantity = targetCartItem.Quantity
			};
		}
	}
}