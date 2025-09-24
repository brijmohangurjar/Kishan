using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;

namespace KrishiClinic.API.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int productId);
        Task<Product> CreateProductAsync(CreateProductDto productDto);
        Task<Product> UpdateProductAsync(int productId, UpdateProductDto productDto);
        Task<bool> DeleteProductAsync(int productId);
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<object>> GetCategoriesAsync();
        Task<int> GetProductCountAsync();
        Task<int> GetLowStockProductCountAsync();
        Task<IEnumerable<Product>> GetAllProductsForAdminAsync();
    }
}
