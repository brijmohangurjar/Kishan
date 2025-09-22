using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KrishiClinic.API.Services;
using KrishiClinic.API.DTOs;
using System.Security.Claims;

namespace KrishiClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            throw new UnauthorizedAccessException("Invalid user token");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCart()
        {
            try
            {
                var userId = GetUserId();
                var cartItems = await _cartService.GetUserCartAsync(userId);
                
                // Convert to DTO to avoid serialization issues
                var cartItemsDto = cartItems.Select(item => new
                {
                    cartId = item.CartId,
                    userId = item.UserId,
                    productId = item.ProductId,
                    quantity = item.Quantity,
                    product = item.Product != null ? new
                    {
                        id = item.Product.ProductId,
                        name = item.Product.Name,
                        description = item.Product.Description,
                        price = item.Product.Price,
                        imageUrl = item.Product.ImageUrl,
                        category = item.Product.Category
                    } : null
                });
                
                return Ok(cartItemsDto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Unauthorized: Invalid user token." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> AddToCart([FromBody] AddToCartDto cartDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                cartDto.UserId = userId; // Override with authenticated user ID
                
                var cartItem = await _cartService.AddToCartAsync(cartDto);
                
                // Convert to DTO
                var cartItemDto = new
                {
                    cartId = cartItem.CartId,
                    userId = cartItem.UserId,
                    productId = cartItem.ProductId,
                    quantity = cartItem.Quantity,
                    product = cartItem.Product != null ? new
                    {
                        id = cartItem.Product.ProductId,
                        name = cartItem.Product.Name,
                        description = cartItem.Product.Description,
                        price = cartItem.Product.Price,
                        imageUrl = cartItem.Product.ImageUrl,
                        category = cartItem.Product.Category
                    } : null
                };
                
                return Ok(cartItemDto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Unauthorized: Invalid user token." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{cartId}")]
        public async Task<ActionResult<object>> UpdateCartItem(int cartId, [FromBody] UpdateCartDto cartDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var cartItem = await _cartService.UpdateCartItemAsync(cartId, cartDto);
                
                // Convert to DTO
                var cartItemDto = new
                {
                    cartId = cartItem.CartId,
                    userId = cartItem.UserId,
                    productId = cartItem.ProductId,
                    quantity = cartItem.Quantity,
                    product = cartItem.Product != null ? new
                    {
                        id = cartItem.Product.ProductId,
                        name = cartItem.Product.Name,
                        description = cartItem.Product.Description,
                        price = cartItem.Product.Price,
                        imageUrl = cartItem.Product.ImageUrl,
                        category = cartItem.Product.Category
                    } : null
                };
                
                return Ok(cartItemDto);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{cartId}")]
        public async Task<ActionResult> RemoveFromCart(int cartId)
        {
            try
            {
                var userId = GetUserId();
                var result = await _cartService.RemoveFromCartAsync(cartId, userId);
                if (!result)
                    return NotFound(new { message = "Cart item not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("clear")]
        public async Task<ActionResult> ClearCart()
        {
            try
            {
                var userId = GetUserId();
                await _cartService.ClearUserCartAsync(userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Unauthorized: Invalid user token." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("total")]
        public async Task<ActionResult<object>> GetCartTotal()
        {
            try
            {
                var userId = GetUserId();
                var total = await _cartService.GetCartTotalAsync(userId);
                return Ok(new { total = total });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Unauthorized: Invalid user token." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
