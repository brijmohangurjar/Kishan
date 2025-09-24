using Microsoft.EntityFrameworkCore;
using KrishiClinic.API.Data;
using KrishiClinic.API.DTOs;
using KrishiClinic.API.Models;

namespace KrishiClinic.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly KrishiClinicDbContext _context;

        public CategoryService(KrishiClinicDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                ProductCount = c.Products.Count
            });
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetActiveCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                ProductCount = c.Products.Count
            });
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return null;

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                ProductCount = category.Products.Count
            };
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                ImageUrl = categoryDto.ImageUrl,
                IsActive = categoryDto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                ProductCount = 0
            };
        }

        public async Task<CategoryResponseDto?> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return null;

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;
            category.ImageUrl = categoryDto.ImageUrl;
            category.IsActive = categoryDto.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                ProductCount = category.Products.Count
            };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            // Check if category has products
            var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
            {
                throw new InvalidOperationException("Cannot delete category that has products. Please move or delete the products first.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleCategoryStatusAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            category.IsActive = !category.IsActive;
            category.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCategoryCountAsync()
        {
            return await _context.Categories.CountAsync();
        }

        public async Task<int> GetActiveCategoryCountAsync()
        {
            return await _context.Categories.CountAsync(c => c.IsActive);
        }
    }
}
