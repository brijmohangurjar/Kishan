using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KrishiClinic.API.Services;
using KrishiClinic.API.DTOs;
using System.Security.Claims;

namespace KrishiClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public ProductsController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            return null; // Return null if not authenticated
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            var userId = GetUserId(); // Get user ID if authenticated
            
            // If user is authenticated, include cart count for each product
            if (userId.HasValue)
            {
                var cartItems = await _cartService.GetUserCartAsync(userId.Value);
                var cartCounts = cartItems.ToDictionary(c => c.ProductId, c => c.Quantity);
                
                var productsWithCartCount = products.Select(p => new
                {
                    productId = p.ProductId,
                    name = p.Name,
                    description = p.Description,
                    price = p.Price,
                    imageUrl = GetFullImageUrl(p.ImageUrl),
                    category = p.Category,
                    cartQuantity = cartCounts.ContainsKey(p.ProductId) ? cartCounts[p.ProductId] : 0
                });
                
                return Ok(productsWithCartCount);
            }
            
            // If not authenticated, return products without cart count
            var productsWithoutCartCount = products.Select(p => new
            {
                productId = p.ProductId,
                name = p.Name,
                description = p.Description,
                price = p.Price,
                imageUrl = GetFullImageUrl(p.ImageUrl),
                category = p.Category,
                cartQuantity = 0
            });
            
            return Ok(productsWithoutCartCount);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            var productDto = new
            {
                productId = product.ProductId,
                name = product.Name,
                description = product.Description,
                price = product.Price,
                imageUrl = GetFullImageUrl(product.ImageUrl),
                category = product.Category,
                stockQuantity = product.StockQuantity,
                isActive = product.IsActive,
                createdAt = product.CreatedAt,
                updatedAt = product.UpdatedAt
            };

            return Ok(productDto);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductsByCategory(string category)
        {
            var products = await _productService.GetProductsByCategoryAsync(category);
            var productsWithFullUrls = products.Select(p => new
            {
                productId = p.ProductId,
                name = p.Name,
                description = p.Description,
                price = p.Price,
                imageUrl = GetFullImageUrl(p.ImageUrl),
                category = p.Category,
                stockQuantity = p.StockQuantity,
                isActive = p.IsActive
            });
            return Ok(productsWithFullUrls);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<object>>> SearchProducts([FromQuery] string q)
        {
            if (string.IsNullOrEmpty(q))
                return BadRequest("Search query is required");

            var products = await _productService.SearchProductsAsync(q);
            var productsWithFullUrls = products.Select(p => new
            {
                productId = p.ProductId,
                name = p.Name,
                description = p.Description,
                price = p.Price,
                imageUrl = GetFullImageUrl(p.ImageUrl),
                category = p.Category,
                stockQuantity = p.StockQuantity,
                isActive = p.IsActive
            });
            return Ok(productsWithFullUrls);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<object>> CreateProduct([FromBody] CreateProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = await _productService.CreateProductAsync(productDto);
                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<object>> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = await _productService.UpdateProductAsync(id, productDto);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<object>>> GetCategories()
        {
            try
            {
                var categories = await _productService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("admin/all")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetAllProductsForAdmin()
        {
            try
            {
                var products = await _productService.GetAllProductsForAdminAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private string GetFullImageUrl(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return string.Empty;

            // If it's already a full URL, return as is
            if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://"))
                return imageUrl;

            // If it's a relative path, prepend the server URL
            if (imageUrl.StartsWith("/"))
                return $"http://localhost:5228{imageUrl}";

            // If it doesn't start with /, assume it's a relative path
            return $"http://localhost:5228/uploads/images/{imageUrl}";
        }
    }
}
