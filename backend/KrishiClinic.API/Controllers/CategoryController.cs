using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KrishiClinic.API.DTOs;
using KrishiClinic.API.Services;

namespace KrishiClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Public endpoint for mobile app to get active categories
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<object>>> GetActiveCategories()
        {
            try
            {
                var categories = await _categoryService.GetActiveCategoriesAsync();
                var categoryDtos = categories.Select(c => new
                {
                    categoryId = c.CategoryId,
                    name = c.Name,
                    description = c.Description,
                    imageUrl = GetFullImageUrl(c.ImageUrl),
                    isActive = c.IsActive,
                    productCount = c.ProductCount
                });

                return Ok(categoryDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetActiveCategories: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        // Admin endpoints
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                var categoryDtos = categories.Select(c => new
                {
                    categoryId = c.CategoryId,
                    name = c.Name,
                    description = c.Description,
                    imageUrl = GetFullImageUrl(c.ImageUrl),
                    isActive = c.IsActive,
                    createdAt = c.CreatedAt,
                    updatedAt = c.UpdatedAt,
                    productCount = c.ProductCount
                });

                return Ok(categoryDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllCategories: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<object>> GetCategory(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                var categoryDto = new
                {
                    categoryId = category.CategoryId,
                    name = category.Name,
                    description = category.Description,
                    imageUrl = GetFullImageUrl(category.ImageUrl),
                    isActive = category.IsActive,
                    createdAt = category.CreatedAt,
                    updatedAt = category.UpdatedAt,
                    productCount = category.ProductCount
                };

                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCategory: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<object>> CreateCategory(CreateCategoryDto categoryDto)
        {
            try
            {
                var category = await _categoryService.CreateCategoryAsync(categoryDto);
                var response = new
                {
                    categoryId = category.CategoryId,
                    name = category.Name,
                    description = category.Description,
                    imageUrl = GetFullImageUrl(category.ImageUrl),
                    isActive = category.IsActive,
                    createdAt = category.CreatedAt,
                    updatedAt = category.UpdatedAt,
                    productCount = category.ProductCount
                };

                return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateCategory: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<object>> UpdateCategory(int id, UpdateCategoryDto categoryDto)
        {
            try
            {
                var category = await _categoryService.UpdateCategoryAsync(id, categoryDto);
                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                var response = new
                {
                    categoryId = category.CategoryId,
                    name = category.Name,
                    description = category.Description,
                    imageUrl = GetFullImageUrl(category.ImageUrl),
                    isActive = category.IsActive,
                    createdAt = category.CreatedAt,
                    updatedAt = category.UpdatedAt,
                    productCount = category.ProductCount
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateCategory: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Category not found" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteCategory: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpPatch("{id}/toggle-status")]
        [Authorize]
        public async Task<ActionResult<object>> ToggleCategoryStatus(int id)
        {
            try
            {
                var result = await _categoryService.ToggleCategoryStatusAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Category not found" });
                }

                var category = await _categoryService.GetCategoryByIdAsync(id);
                var response = new
                {
                    categoryId = category.CategoryId,
                    name = category.Name,
                    description = category.Description,
                    imageUrl = GetFullImageUrl(category.ImageUrl),
                    isActive = category.IsActive,
                    createdAt = category.CreatedAt,
                    updatedAt = category.UpdatedAt,
                    productCount = category.ProductCount
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ToggleCategoryStatus: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("stats")]
        [Authorize]
        public async Task<ActionResult<object>> GetCategoryStats()
        {
            try
            {
                var totalCategories = await _categoryService.GetCategoryCountAsync();
                var activeCategories = await _categoryService.GetActiveCategoryCountAsync();

                return Ok(new
                {
                    totalCategories,
                    activeCategories,
                    inactiveCategories = totalCategories - activeCategories
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCategoryStats: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        private string GetFullImageUrl(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return string.Empty;

            if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://"))
                return imageUrl;

            if (imageUrl.StartsWith("/"))
                return $"http://localhost:5228{imageUrl}";

            return $"http://localhost:5228/uploads/categories/{imageUrl}";
        }
    }
}