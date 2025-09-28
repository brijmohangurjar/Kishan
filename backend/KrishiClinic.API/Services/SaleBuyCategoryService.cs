using Microsoft.EntityFrameworkCore;
using KrishiClinic.API.Data;
using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;

namespace KrishiClinic.API.Services
{
    public interface ISaleBuyCategoryService
    {
        Task<IEnumerable<SaleBuyCategory>> GetAllCategoriesAsync();
        Task<IEnumerable<SaleBuyCategory>> GetActiveCategoriesAsync();
        Task<SaleBuyCategory?> GetCategoryByIdAsync(int id);
        Task<SaleBuyCategory> CreateCategoryAsync(CreateSaleBuyCategoryDto dto, string imageUrl);
        Task<SaleBuyCategory?> UpdateCategoryAsync(int id, UpdateSaleBuyCategoryDto dto, string? imageUrl);
        Task<bool> DeleteCategoryAsync(int id);
    }

    public class SaleBuyCategoryService : ISaleBuyCategoryService
    {
        private readonly KrishiClinicDbContext _context;

        public SaleBuyCategoryService(KrishiClinicDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SaleBuyCategory>> GetAllCategoriesAsync()
        {
            return await _context.SaleBuyCategories
                .Include(c => c.SaleBuyProducts)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<SaleBuyCategory>> GetActiveCategoriesAsync()
        {
            return await _context.SaleBuyCategories
                .Include(c => c.SaleBuyProducts.Where(p => p.IsActive))
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<SaleBuyCategory?> GetCategoryByIdAsync(int id)
        {
            return await _context.SaleBuyCategories
                .Include(c => c.SaleBuyProducts)
                .FirstOrDefaultAsync(c => c.SaleBuyCategoryId == id);
        }

        public async Task<SaleBuyCategory> CreateCategoryAsync(CreateSaleBuyCategoryDto dto, string imageUrl)
        {
            var category = new SaleBuyCategory
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = imageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.SaleBuyCategories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<SaleBuyCategory?> UpdateCategoryAsync(int id, UpdateSaleBuyCategoryDto dto, string? imageUrl)
        {
            var category = await _context.SaleBuyCategories.FindAsync(id);
            if (category == null) return null;

            category.Name = dto.Name;
            category.Description = dto.Description;
            category.IsActive = dto.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(imageUrl))
            {
                category.ImageUrl = imageUrl;
            }

            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.SaleBuyCategories.FindAsync(id);
            if (category == null) return false;

            _context.SaleBuyCategories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
