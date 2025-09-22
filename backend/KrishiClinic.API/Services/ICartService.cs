using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;

namespace KrishiClinic.API.Services
{
    public interface ICartService
    {
        Task<IEnumerable<Cart>> GetUserCartAsync(int userId);
        Task<Cart> AddToCartAsync(AddToCartDto cartDto);
        Task<Cart> UpdateCartItemAsync(int cartId, UpdateCartDto cartDto);
        Task<bool> RemoveFromCartAsync(int cartId, int userId);
        Task<bool> ClearUserCartAsync(int userId);
        Task<decimal> GetCartTotalAsync(int userId);
    }
}
