using Microsoft.AspNetCore.Mvc;
using KrishiClinic.API.Data;
using Microsoft.EntityFrameworkCore;

namespace KrishiClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly KrishiClinicDbContext _context;

        public TestController(KrishiClinicDbContext context)
        {
            _context = context;
        }

        [HttpGet("debug-categories")]
        public async Task<ActionResult> DebugCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            
            return Ok(new
            {
                totalCategories = categories.Count,
                categories = categories.Select(c => new
                {
                    c.CategoryId,
                    c.Name,
                    c.IsActive,
                    ProductCount = c.Products.Count
                }),
                // Test the same query that the mobile app uses
                activeCategories = categories.Where(c => c.IsActive).Select(c => new
                {
                    categoryId = c.CategoryId,
                    name = c.Name,
                    description = c.Description,
                    imageUrl = c.ImageUrl,
                    isActive = c.IsActive,
                    productCount = c.Products.Count
                })
            });
        }

        [HttpPost("create-test-categories")]
        public async Task<ActionResult> CreateTestCategories()
        {
            var existingCategories = await _context.Categories.ToListAsync();
            
            if (existingCategories.Any())
            {
                return Ok(new { message = "Categories already exist", count = existingCategories.Count });
            }
            
            var testCategories = new[]
            {
                new KrishiClinic.API.Models.Category
                {
                    Name = "मूंग",
                    Description = "Mung beans and related products",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new KrishiClinic.API.Models.Category
                {
                    Name = "गेहूं",
                    Description = "Wheat and wheat products",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new KrishiClinic.API.Models.Category
                {
                    Name = "चावल",
                    Description = "Rice and rice products",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new KrishiClinic.API.Models.Category
                {
                    Name = "सब्जियां",
                    Description = "Fresh vegetables",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };
            
            _context.Categories.AddRange(testCategories);
            await _context.SaveChangesAsync();
            
            return Ok(new { 
                message = $"Created {testCategories.Length} test categories",
                categories = testCategories.Select(c => new { c.CategoryId, c.Name })
            });
        }

        [HttpGet("check-categories")]
        public async Task<ActionResult> CheckCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            
            return Ok(new
            {
                totalCategories = categories.Count,
                categories = categories.Select(c => new
                {
                    c.CategoryId,
                    c.Name,
                    c.IsActive,
                    ProductCount = c.Products.Count,
                    Products = c.Products.Select(p => new { p.ProductId, p.Name, p.CategoryId })
                })
            });
        }

        [HttpGet("check-products")]
        public async Task<ActionResult> CheckProducts()
        {
            var products = await _context.Products.ToListAsync();
            var categories = await _context.Categories.ToListAsync();
            
            return Ok(new
            {
                products = products.Select(p => new
                {
                    p.ProductId,
                    p.Name,
                    p.Category,
                    p.CategoryId,
                    CategoryName = p.CategoryNavigation?.Name
                }),
                categories = categories.Select(c => new
                {
                    c.CategoryId,
                    c.Name,
                    c.IsActive
                })
            });
        }

        [HttpGet("test-category-filter/{category}")]
        public async Task<ActionResult> TestCategoryFilter(string category)
        {
            var products = await _context.Products
                .Include(p => p.CategoryNavigation)
                .Where(p => p.IsActive)
                .ToListAsync();
            
            var categories = await _context.Categories.ToListAsync();
            
            // Test different matching strategies
            var exactMatch = categories.FirstOrDefault(c => c.Name == category);
            var trimMatch = categories.FirstOrDefault(c => c.Name.Trim() == category.Trim());
            var caseInsensitiveMatch = categories.FirstOrDefault(c => c.Name.Trim().ToLower() == category.Trim().ToLower());
            
            var productsByCategoryId = products.Where(p => p.CategoryId != null).ToList();
            var productsByCategoryName = products.Where(p => !string.IsNullOrEmpty(p.Category)).ToList();
            
            return Ok(new
            {
                searchCategory = category,
                searchCategoryLength = category.Length,
                searchCategoryBytes = System.Text.Encoding.UTF8.GetBytes(category),
                exactMatch = exactMatch != null ? new { exactMatch.CategoryId, exactMatch.Name, exactMatch.Name.Length } : null,
                trimMatch = trimMatch != null ? new { trimMatch.CategoryId, trimMatch.Name, trimMatch.Name.Length } : null,
                caseInsensitiveMatch = caseInsensitiveMatch != null ? new { caseInsensitiveMatch.CategoryId, caseInsensitiveMatch.Name, caseInsensitiveMatch.Name.Length } : null,
                allCategories = categories.Select(c => new { c.CategoryId, c.Name, NameLength = c.Name.Length, TrimmedLength = c.Name.Trim().Length }),
                productsWithCategoryId = productsByCategoryId.Count,
                productsWithCategoryName = productsByCategoryName.Count,
                productsByCategoryIdList = productsByCategoryId.Select(p => new { p.ProductId, p.Name, p.CategoryId, CategoryName = p.CategoryNavigation?.Name }),
                productsByCategoryNameList = productsByCategoryName.Select(p => new { p.ProductId, p.Name, p.Category, p.CategoryId })
            });
        }

        [HttpPost("fix-categories")]
        public async Task<ActionResult> FixCategories()
        {
            var products = await _context.Products.ToListAsync();
            var categories = await _context.Categories.ToListAsync();
            
            int updated = 0;
            var details = new List<string>();
            
            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.Category))
                {
                    // Try exact match first
                    var category = categories.FirstOrDefault(c => 
                        c.Name.Equals(product.Category, StringComparison.OrdinalIgnoreCase));
                    
                    // If no exact match, try trimming whitespace
                    if (category == null)
                    {
                        category = categories.FirstOrDefault(c => 
                            c.Name.Trim().Equals(product.Category.Trim(), StringComparison.OrdinalIgnoreCase));
                    }
                    
                    if (category != null)
                    {
                        product.CategoryId = category.CategoryId;
                        updated++;
                        details.Add($"Product '{product.Name}' linked to category '{category.Name}' (ID: {category.CategoryId})");
                    }
                    else
                    {
                        details.Add($"No matching category found for product '{product.Name}' with category '{product.Category}'");
                    }
                }
            }
            
            await _context.SaveChangesAsync();
            
            return Ok(new { 
                message = $"Updated {updated} products with CategoryId",
                details = details
            });
        }
    }
}