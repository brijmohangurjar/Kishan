using Microsoft.EntityFrameworkCore;
using KrishiClinic.API.Data;
using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;

namespace KrishiClinic.API.Services
{
    public class CartService : ICartService
    {
        private readonly KrishiClinicDbContext _context;

        public CartService(KrishiClinicDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cart>> GetUserCartAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Cart> AddToCartAsync(AddToCartDto cartDto)
        {
            var existingCartItem = await _context.Carts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.UserId == cartDto.UserId && c.ProductId == cartDto.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cartDto.Quantity;
                existingCartItem.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return existingCartItem;
            }

            var cartItem = new Cart
            {
                UserId = cartDto.UserId,
                ProductId = cartDto.ProductId,
                Quantity = cartDto.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            _context.Carts.Add(cartItem);
            await _context.SaveChangesAsync();
            
            // Load the cart item with product for return
            return await _context.Carts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartItem.CartId);
        }

        public async Task<Cart> UpdateCartItemAsync(int cartId, UpdateCartDto cartDto)
        {
            var cartItem = await _context.Carts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
            if (cartItem == null)
                throw new ArgumentException("Cart item not found");

            cartItem.Quantity = cartDto.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<bool> RemoveFromCartAsync(int cartId, int userId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem == null || cartItem.UserId != userId)
                return false;

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearUserCartAsync(int userId)
        {
            var cartItems = await _context.Carts.Where(c => c.UserId == userId).ToListAsync();
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Product.Price * c.Quantity);
        }
    }
}
