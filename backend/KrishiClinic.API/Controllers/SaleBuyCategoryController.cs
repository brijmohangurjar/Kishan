using Microsoft.AspNetCore.Mvc;
using KrishiClinic.API.Services;
using KrishiClinic.API.DTOs;
using KrishiClinic.API.Models;

namespace KrishiClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleBuyCategoryController : ControllerBase
    {
        private readonly ISaleBuyCategoryService _saleBuyCategoryService;
        private readonly IFileUploadService _fileUploadService;

        public SaleBuyCategoryController(ISaleBuyCategoryService saleBuyCategoryService, IFileUploadService fileUploadService)
        {
            _saleBuyCategoryService = saleBuyCategoryService;
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllCategories()
        {
            var categories = await _saleBuyCategoryService.GetAllCategoriesAsync();
            var categoriesWithFullUrls = categories.Select(c => new
            {
                saleBuyCategoryId = c.SaleBuyCategoryId,
                name = c.Name,
                description = c.Description,
                imageUrl = GetFullImageUrl(c.ImageUrl),
                isActive = c.IsActive,
                productCount = c.SaleBuyProducts.Count,
                createdAt = c.CreatedAt
            });
            return Ok(categoriesWithFullUrls);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<object>>> GetActiveCategories()
        {
            var categories = await _saleBuyCategoryService.GetActiveCategoriesAsync();
            var categoriesWithFullUrls = categories.Select(c => new
            {
                saleBuyCategoryId = c.SaleBuyCategoryId,
                name = c.Name,
                description = c.Description,
                imageUrl = GetFullImageUrl(c.ImageUrl),
                isActive = c.IsActive,
                productCount = c.SaleBuyProducts.Count,
                createdAt = c.CreatedAt
            });
            return Ok(categoriesWithFullUrls);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetCategoryById(int id)
        {
            var category = await _saleBuyCategoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(new
            {
                saleBuyCategoryId = category.SaleBuyCategoryId,
                name = category.Name,
                description = category.Description,
                imageUrl = GetFullImageUrl(category.ImageUrl),
                isActive = category.IsActive,
                productCount = category.SaleBuyProducts.Count,
                createdAt = category.CreatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateCategory([FromBody] CreateSaleBuyCategoryDto dto)
        {
            try
            {
                string? imageUrl = null;
                if (dto.ImageFile != null)
                {
                    imageUrl = await _fileUploadService.UploadImageAsync(dto.ImageFile);
                }
                else if (!string.IsNullOrEmpty(dto.ImageUrl))
                {
                    imageUrl = dto.ImageUrl;
                }

                var category = await _saleBuyCategoryService.CreateCategoryAsync(dto, imageUrl ?? "");

                return CreatedAtAction(nameof(GetCategoryById), new { id = category.SaleBuyCategoryId }, new
                {
                    saleBuyCategoryId = category.SaleBuyCategoryId,
                    name = category.Name,
                    description = category.Description,
                    imageUrl = GetFullImageUrl(category.ImageUrl),
                    isActive = category.IsActive,
                    productCount = 0,
                    createdAt = category.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateCategory(int id, [FromBody] UpdateSaleBuyCategoryDto dto)
        {
            try
            {
                string? imageUrl = dto.ImageUrl;
                
                // If ImageFile is provided, upload it and use the new URL
                if (dto.ImageFile != null)
                {
                    imageUrl = await _fileUploadService.UploadImageAsync(dto.ImageFile);
                }

                var category = await _saleBuyCategoryService.UpdateCategoryAsync(id, dto, imageUrl);
                if (category == null)
                    return NotFound();

                return Ok(new
                {
                    saleBuyCategoryId = category.SaleBuyCategoryId,
                    name = category.Name,
                    description = category.Description,
                    imageUrl = GetFullImageUrl(category.ImageUrl),
                    isActive = category.IsActive,
                    productCount = category.SaleBuyProducts.Count,
                    createdAt = category.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var result = await _saleBuyCategoryService.DeleteCategoryAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        private string GetFullImageUrl(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return "";

            if (imageUrl.StartsWith("http"))
                return imageUrl;

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            
            // If it already starts with /uploads/, just prepend the base URL
            if (imageUrl.StartsWith("/uploads/"))
                return $"{baseUrl}{imageUrl}";
            
            // If it doesn't start with /, assume it's just a filename
            return $"{baseUrl}/uploads/images/{imageUrl}";
        }
    }
}
