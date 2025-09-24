using Microsoft.EntityFrameworkCore;
using KrishiClinic.API.Data;
using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;

namespace KrishiClinic.API.Services
{
    public class ProductService : IProductService
    {
        private readonly KrishiClinicDbContext _context;

        public ProductService(KrishiClinicDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Where(p => p.IsActive).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products.FindAsync(productId);
        }

        public async Task<Product> CreateProductAsync(CreateProductDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                ImageUrl = productDto.ImageUrl,
                AdditionalImageUrls = productDto.AdditionalImageUrls,
                Category = productDto.Category,
                StockQuantity = productDto.StockQuantity,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(int productId, UpdateProductDto productDto)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new ArgumentException("Product not found");

            if (!string.IsNullOrEmpty(productDto.Name))
                product.Name = productDto.Name;
            if (!string.IsNullOrEmpty(productDto.Description))
                product.Description = productDto.Description;
            if (productDto.Price.HasValue)
                product.Price = productDto.Price.Value;
            if (!string.IsNullOrEmpty(productDto.ImageUrl))
                product.ImageUrl = productDto.ImageUrl;
            if (productDto.AdditionalImageUrls != null)
                product.AdditionalImageUrls = productDto.AdditionalImageUrls;
            if (!string.IsNullOrEmpty(productDto.Category))
                product.Category = productDto.Category;
            if (productDto.StockQuantity.HasValue)
                product.StockQuantity = productDto.StockQuantity.Value;
            if (productDto.IsActive.HasValue)
                product.IsActive = productDto.IsActive.Value;

            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.CategoryNavigation)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            // First try to find by CategoryId (new relationship)
            // Try exact match first
            var categoryEntity = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == category && c.IsActive);
            
            // If no exact match, try trimming whitespace
            if (categoryEntity == null)
            {
                categoryEntity = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.Trim() == category.Trim() && c.IsActive);
            }
            
            // If still no match, try case-insensitive comparison
            if (categoryEntity == null)
            {
                categoryEntity = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.Trim().ToLower() == category.Trim().ToLower() && c.IsActive);
            }
            
            if (categoryEntity != null)
            {
                // Use the new CategoryId relationship
                return await _context.Products
                    .Include(p => p.CategoryNavigation)
                    .Where(p => p.CategoryId == categoryEntity.CategoryId && p.IsActive)
                    .ToListAsync();
            }
            else
            {
                // Fallback to old Category field for backward compatibility
                // Try exact match first
                var products = await _context.Products
                    .Include(p => p.CategoryNavigation)
                    .Where(p => p.Category == category && p.IsActive)
                    .ToListAsync();
                
                // If no products found, try trimming whitespace
                if (!products.Any())
                {
                    products = await _context.Products
                        .Include(p => p.CategoryNavigation)
                        .Where(p => p.Category.Trim() == category.Trim() && p.IsActive)
                        .ToListAsync();
                }
                
                // If still no products, try case-insensitive comparison
                if (!products.Any())
                {
                    products = await _context.Products
                        .Include(p => p.CategoryNavigation)
                        .Where(p => p.Category.Trim().ToLower() == category.Trim().ToLower() && p.IsActive)
                        .ToListAsync();
                }
                
                return products;
            }
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Where(p => p.IsActive && 
                    (p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetCategoriesAsync()
        {
            var categories = await _context.Products
                .Where(p => p.IsActive)
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return categories.Select(c => new { name = c });
        }

        public async Task<int> GetProductCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<int> GetLowStockProductCountAsync()
        {
            return await _context.Products.CountAsync(p => p.StockQuantity <= 5);
        }

        public async Task<IEnumerable<Product>> GetAllProductsForAdminAsync()
        {
            return await _context.Products
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
