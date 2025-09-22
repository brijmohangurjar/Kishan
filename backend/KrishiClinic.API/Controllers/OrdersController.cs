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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            throw new UnauthorizedAccessException("Invalid user token");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUserOrders()
        {
            try
            {
                var userId = GetUserId();
                var orders = await _orderService.GetUserOrdersAsync(userId);
                return Ok(orders);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<object>> GetOrder(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                    return NotFound(new { message = "Order not found" });

                // Check if the order belongs to the authenticated user
                var userId = GetUserId();
                if (order.UserId != userId)
                    return Forbid();

                return Ok(order);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                orderDto.UserId = userId; // Override with authenticated user ID
                
                var order = await _orderService.CreateOrderAsync(orderDto);
                return CreatedAtAction(nameof(GetOrder), new { orderId = order.OrderId }, order);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{orderId}/status")]
        public async Task<ActionResult<object>> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto statusDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                var order = await _orderService.GetOrderByIdAsync(orderId);
                
                if (order == null)
                    return NotFound(new { message = "Order not found" });

                // Check if the order belongs to the authenticated user
                if (order.UserId != userId)
                    return Forbid();

                var updatedOrder = await _orderService.UpdateOrderStatusAsync(orderId, statusDto);
                return Ok(updatedOrder);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{orderId}")]
        public async Task<ActionResult> DeleteOrder(int orderId)
        {
            try
            {
                var userId = GetUserId();
                var order = await _orderService.GetOrderByIdAsync(orderId);
                
                if (order == null)
                    return NotFound(new { message = "Order not found" });

                // Check if the order belongs to the authenticated user
                if (order.UserId != userId)
                    return Forbid();

                var result = await _orderService.DeleteOrderAsync(orderId);
                if (!result)
                    return NotFound(new { message = "Order not found" });

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{orderId}/total")]
        public async Task<ActionResult<object>> GetOrderTotal(int orderId)
        {
            try
            {
                var userId = GetUserId();
                var order = await _orderService.GetOrderByIdAsync(orderId);
                
                if (order == null)
                    return NotFound(new { message = "Order not found" });

                // Check if the order belongs to the authenticated user
                if (order.UserId != userId)
                    return Forbid();

                var total = await _orderService.GetOrderTotalAsync(orderId);
                return Ok(new { total = total });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

