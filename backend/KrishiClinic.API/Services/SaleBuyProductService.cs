using Microsoft.EntityFrameworkCore;
using KrishiClinic.API.Data;
using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;
using System.Text.Json;

namespace KrishiClinic.API.Services
{
    public interface ISaleBuyProductService
    {
        Task<IEnumerable<SaleBuyProduct>> GetAllProductsAsync();
        Task<IEnumerable<SaleBuyProduct>> GetActiveProductsAsync();
        Task<IEnumerable<SaleBuyProduct>> GetProductsByCategoryIdAsync(int categoryId);
        Task<SaleBuyProduct?> GetProductByIdAsync(int id);
        Task<SaleBuyProduct> CreateProductAsync(CreateSaleBuyProductDto dto, int userId, List<string> imageUrls);
        Task<SaleBuyProduct?> UpdateProductAsync(int id, UpdateSaleBuyProductDto dto, int userId, List<string>? imageUrls);
        Task<bool> DeleteProductAsync(int id, int userId);
        Task<bool> CanUserEditProductAsync(int productId, int userId);
    }

    public class SaleBuyProductService : ISaleBuyProductService
    {
        private readonly KrishiClinicDbContext _context;

        public SaleBuyProductService(KrishiClinicDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SaleBuyProduct>> GetAllProductsAsync()
        {
            return await _context.SaleBuyProducts
                .Include(p => p.SaleBuyCategory)
                .Include(p => p.CreatedByUser)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SaleBuyProduct>> GetActiveProductsAsync()
        {
            return await _context.SaleBuyProducts
                .Include(p => p.SaleBuyCategory)
                .Include(p => p.CreatedByUser)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SaleBuyProduct>> GetProductsByCategoryIdAsync(int categoryId)
        {
            return await _context.SaleBuyProducts
                .Include(p => p.SaleBuyCategory)
                .Include(p => p.CreatedByUser)
                .Where(p => p.SaleBuyCategoryId == categoryId && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<SaleBuyProduct?> GetProductByIdAsync(int id)
        {
            return await _context.SaleBuyProducts
                .Include(p => p.SaleBuyCategory)
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(p => p.SaleBuyProductId == id);
        }

        public async Task<SaleBuyProduct> CreateProductAsync(CreateSaleBuyProductDto dto, int userId, List<string> imageUrls)
        {
            var product = new SaleBuyProduct
            {
                FullName = dto.FullName,
                PlaceName = dto.PlaceName,
                ProductDescription = dto.ProductDescription,
                Price = dto.Price,
                PhoneNumber = dto.PhoneNumber,
                SaleBuyCategoryId = dto.SaleBuyCategoryId,
                CreatedByUserId = userId,
                ImageUrls = JsonSerializer.Serialize(imageUrls),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.SaleBuyProducts.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<SaleBuyProduct?> UpdateProductAsync(int id, UpdateSaleBuyProductDto dto, int userId, List<string>? imageUrls)
        {
            var product = await _context.SaleBuyProducts.FindAsync(id);
            if (product == null || product.CreatedByUserId != userId) return null;

            product.FullName = dto.FullName;
            product.PlaceName = dto.PlaceName;
            product.ProductDescription = dto.ProductDescription;
            product.Price = dto.Price;
            product.PhoneNumber = dto.PhoneNumber;
            product.SaleBuyCategoryId = dto.SaleBuyCategoryId;
            product.IsActive = dto.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            if (imageUrls != null && imageUrls.Any())
            {
                product.ImageUrls = JsonSerializer.Serialize(imageUrls);
            }

            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id, int userId)
        {
            var product = await _context.SaleBuyProducts.FindAsync(id);
            if (product == null || product.CreatedByUserId != userId) return false;

            _context.SaleBuyProducts.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanUserEditProductAsync(int productId, int userId)
        {
            var product = await _context.SaleBuyProducts.FindAsync(productId);
            return product != null && product.CreatedByUserId == userId;
        }
    }
}
