using Microsoft.AspNetCore.Mvc;
using KrishiClinic.API.Services;
using KrishiClinic.API.DTOs;
using KrishiClinic.API.Models;
using System.Text.Json;
using System.Security.Claims;

namespace KrishiClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleBuyProductController : ControllerBase
    {
        private readonly ISaleBuyProductService _saleBuyProductService;
        private readonly IFileUploadService _fileUploadService;

        public SaleBuyProductController(ISaleBuyProductService saleBuyProductService, IFileUploadService fileUploadService)
        {
            _saleBuyProductService = saleBuyProductService;
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllProducts()
        {
            var products = await _saleBuyProductService.GetAllProductsAsync();
            var productsWithFullUrls = products.Select(p => new
            {
                saleBuyProductId = p.SaleBuyProductId,
                fullName = p.FullName,
                placeName = p.PlaceName,
                productDescription = p.ProductDescription,
                price = p.Price,
                phoneNumber = p.PhoneNumber,
                imageUrls = GetImageUrls(p.ImageUrls),
                isActive = p.IsActive,
                createdAt = p.CreatedAt,
                saleBuyCategoryId = p.SaleBuyCategoryId,
                saleBuyCategoryName = p.SaleBuyCategory?.Name,
                createdByUserId = p.CreatedByUserId,
                createdByUserName = p.CreatedByUser?.Name,
                canEdit = false // Will be set based on current user
            });
            return Ok(productsWithFullUrls);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<object>>> GetActiveProducts()
        {
            var products = await _saleBuyProductService.GetActiveProductsAsync();
            var currentUserId = GetCurrentUserId();
            
            var productsWithFullUrls = products.Select(p => new
            {
                saleBuyProductId = p.SaleBuyProductId,
                fullName = p.FullName,
                placeName = p.PlaceName,
                productDescription = p.ProductDescription,
                price = p.Price,
                phoneNumber = p.PhoneNumber,
                imageUrls = GetImageUrls(p.ImageUrls),
                isActive = p.IsActive,
                createdAt = p.CreatedAt,
                saleBuyCategoryId = p.SaleBuyCategoryId,
                saleBuyCategoryName = p.SaleBuyCategory?.Name,
                createdByUserId = p.CreatedByUserId,
                createdByUserName = p.CreatedByUser?.Name,
                canEdit = currentUserId.HasValue && p.CreatedByUserId == currentUserId.Value
            });
            return Ok(productsWithFullUrls);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductsByCategoryId(int categoryId)
        {
            var products = await _saleBuyProductService.GetProductsByCategoryIdAsync(categoryId);
            var currentUserId = GetCurrentUserId();
            
            var productsWithFullUrls = products.Select(p => new
            {
                saleBuyProductId = p.SaleBuyProductId,
                fullName = p.FullName,
                placeName = p.PlaceName,
                productDescription = p.ProductDescription,
                price = p.Price,
                phoneNumber = p.PhoneNumber,
                imageUrls = GetImageUrls(p.ImageUrls),
                isActive = p.IsActive,
                createdAt = p.CreatedAt,
                saleBuyCategoryId = p.SaleBuyCategoryId,
                saleBuyCategoryName = p.SaleBuyCategory?.Name,
                createdByUserId = p.CreatedByUserId,
                createdByUserName = p.CreatedByUser?.Name,
                canEdit = currentUserId.HasValue && p.CreatedByUserId == currentUserId.Value
            });
            return Ok(productsWithFullUrls);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProductById(int id)
        {
            var product = await _saleBuyProductService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            var currentUserId = GetCurrentUserId();

            return Ok(new
            {
                saleBuyProductId = product.SaleBuyProductId,
                fullName = product.FullName,
                placeName = product.PlaceName,
                productDescription = product.ProductDescription,
                price = product.Price,
                phoneNumber = product.PhoneNumber,
                imageUrls = GetImageUrls(product.ImageUrls),
                isActive = product.IsActive,
                createdAt = product.CreatedAt,
                saleBuyCategoryId = product.SaleBuyCategoryId,
                saleBuyCategoryName = product.SaleBuyCategory?.Name,
                createdByUserId = product.CreatedByUserId,
                createdByUserName = product.CreatedByUser?.Name,
                canEdit = currentUserId.HasValue && product.CreatedByUserId == currentUserId.Value
            });
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateProduct([FromBody] CreateSaleBuyProductDto dto)
        {
            try
            {
                // Debug logging
                Console.WriteLine($"Received FullName: '{dto.FullName}'");
                Console.WriteLine($"Received PlaceName: '{dto.PlaceName}'");
                Console.WriteLine($"Received ProductDescription: '{dto.ProductDescription}'");
                Console.WriteLine($"Received Price: '{dto.Price}'");
                Console.WriteLine($"Received PhoneNumber: '{dto.PhoneNumber}'");
                Console.WriteLine($"Received SaleBuyCategoryId: '{dto.SaleBuyCategoryId}'");
                Console.WriteLine($"Received ImageFiles count: {dto.ImageFiles?.Count ?? 0}");

                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue)
                    return Unauthorized(new { message = "User must be authenticated to create products" });

                List<string> imageUrls = new List<string>();
                
                // Handle ImageUrls (pre-uploaded images)
                if (dto.ImageUrls != null && dto.ImageUrls.Any())
                {
                    imageUrls.AddRange(dto.ImageUrls);
                }
                
                // Handle ImageFiles (new file uploads)
                if (dto.ImageFiles != null && dto.ImageFiles.Any())
                {
                    foreach (var file in dto.ImageFiles)
                    {
                        var imageUrl = await _fileUploadService.UploadImageAsync(file);
                        imageUrls.Add(imageUrl);
                    }
                }

                var product = await _saleBuyProductService.CreateProductAsync(dto, currentUserId.Value, imageUrls);

                return CreatedAtAction(nameof(GetProductById), new { id = product.SaleBuyProductId }, new
                {
                    saleBuyProductId = product.SaleBuyProductId,
                    fullName = product.FullName,
                    placeName = product.PlaceName,
                    productDescription = product.ProductDescription,
                    price = product.Price,
                    phoneNumber = product.PhoneNumber,
                    imageUrls = GetImageUrls(product.ImageUrls),
                    isActive = product.IsActive,
                    createdAt = product.CreatedAt,
                    saleBuyCategoryId = product.SaleBuyCategoryId,
                    saleBuyCategoryName = product.SaleBuyCategory?.Name,
                    createdByUserId = product.CreatedByUserId,
                    createdByUserName = product.CreatedByUser?.Name,
                    canEdit = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateProduct(int id, [FromBody] UpdateSaleBuyProductDto dto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue)
                    return Unauthorized(new { message = "User must be authenticated to update products" });

                List<string>? imageUrls = null;
                if (dto.ImageFiles != null && dto.ImageFiles.Any())
                {
                    imageUrls = new List<string>();
                    foreach (var file in dto.ImageFiles)
                    {
                        var imageUrl = await _fileUploadService.UploadImageAsync(file);
                        imageUrls.Add(imageUrl);
                    }
                }

                var product = await _saleBuyProductService.UpdateProductAsync(id, dto, currentUserId.Value, imageUrls);
                if (product == null)
                    return NotFound(new { message = "Product not found or you don't have permission to edit it" });

                return Ok(new
                {
                    saleBuyProductId = product.SaleBuyProductId,
                    fullName = product.FullName,
                    placeName = product.PlaceName,
                    productDescription = product.ProductDescription,
                    price = product.Price,
                    phoneNumber = product.PhoneNumber,
                    imageUrls = GetImageUrls(product.ImageUrls),
                    isActive = product.IsActive,
                    createdAt = product.CreatedAt,
                    saleBuyCategoryId = product.SaleBuyCategoryId,
                    saleBuyCategoryName = product.SaleBuyCategory?.Name,
                    createdByUserId = product.CreatedByUserId,
                    createdByUserName = product.CreatedByUser?.Name,
                    canEdit = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
                return Unauthorized(new { message = "User must be authenticated to delete products" });

            var result = await _saleBuyProductService.DeleteProductAsync(id, currentUserId.Value);
            if (!result)
                return NotFound(new { message = "Product not found or you don't have permission to delete it" });

            return NoContent();
        }

        private List<string> GetImageUrls(string? imageUrlsJson)
        {
            if (string.IsNullOrEmpty(imageUrlsJson))
                return new List<string>();

            try
            {
                var urls = JsonSerializer.Deserialize<List<string>>(imageUrlsJson);
                return urls ?? new List<string>(); // Return original URLs without adding base URL
            }
            catch
            {
                return new List<string>();
            }
        }

        private string GetFullImageUrl(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return "";

            if (imageUrl.StartsWith("http"))
                return imageUrl;

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return $"{baseUrl}/uploads/images/{imageUrl}";
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
