using KrishiClinic.API.DTOs;
using KrishiClinic.API.Models;

namespace KrishiClinic.API.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<IEnumerable<CategoryResponseDto>> GetActiveCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id);
        Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto categoryDto);
        Task<CategoryResponseDto?> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> ToggleCategoryStatusAsync(int id);
        Task<int> GetCategoryCountAsync();
        Task<int> GetActiveCategoryCountAsync();
    }
}
