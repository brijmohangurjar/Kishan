using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KrishiClinic.API.Services;
using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;
using System.Security.Claims;

namespace KrishiClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public AdminController(
            IAdminService adminService,
            IProductService productService,
            IUserService userService,
            IOrderService orderService)
        {
            _adminService = adminService;
            _productService = productService;
            _userService = userService;
            _orderService = orderService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] AdminLoginRequest request)
        {
            try
            {
                var result = await _adminService.LoginAsync(request.Email, request.Password);
                if (result == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<object>> CreateAdmin([FromBody] CreateAdminDto adminDto)
        {
            try
            {
                // Check if admin already exists
                var existingAdmin = await _adminService.GetAdminByEmailAsync(adminDto.Email);
                if (existingAdmin != null)
                    return Conflict(new { message = "Admin with this email already exists" });

                var admin = await _adminService.CreateAdminAsync(adminDto);
                
                return CreatedAtAction(nameof(CreateAdmin), new { id = admin.AdminId }, new { 
                    message = "Admin created successfully",
                    admin = new {
                        adminId = admin.AdminId,
                        name = admin.Name,
                        email = admin.Email,
                        role = admin.Role,
                        isActive = admin.IsActive,
                        createdAt = admin.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create-brijmohan")]
        public async Task<ActionResult<object>> CreateBrijmohanAdmin()
        {
            try
            {
                var adminDto = new CreateAdminDto
                {
                    Name = "Brijmohan Gurjar",
                    Email = "brijmohangurjar48@gmail.com",
                    Password = "Admin@123",
                    Role = "SuperAdmin"
                };

                // Check if admin already exists
                var existingAdmin = await _adminService.GetAdminByEmailAsync(adminDto.Email);
                if (existingAdmin != null)
                    return Ok(new { message = "Admin already exists", admin = existingAdmin });

                var admin = await _adminService.CreateAdminAsync(adminDto);
                
                return Ok(new { 
                    message = "Brijmohan admin created successfully",
                    admin = new {
                        adminId = admin.AdminId,
                        name = admin.Name,
                        email = admin.Email,
                        role = admin.Role,
                        isActive = admin.IsActive,
                        createdAt = admin.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("users")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("users/{id}")]
        [Authorize]
        public async Task<ActionResult<object>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("users/{id}/status")]
        [Authorize]
        public async Task<ActionResult> UpdateUserStatus(int id, [FromBody] UpdateUserStatusRequest request)
        {
            try
            {
                var success = await _userService.UpdateUserStatusAsync(id, request.IsActive);
                if (!success)
                    return NotFound(new { message = "User not found" });

                return Ok(new { message = "User status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("orders")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("orders/{id}")]
        [Authorize]
        public async Task<ActionResult<object>> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound(new { message = "Order not found" });

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("orders/{id}/status")]
        [Authorize]
        public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                var success = await _orderService.UpdateOrderStatusAsync(id, request.Status, request.ShippedDate, request.DeliveredDate);
                if (!success)
                    return NotFound(new { message = "Order not found" });

                return Ok(new { message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("dashboard/stats")]
        [Authorize]
        public async Task<ActionResult<object>> GetDashboardStats()
        {
            try
            {
                var stats = new
                {
                    totalProducts = await _productService.GetProductCountAsync(),
                    totalUsers = await _userService.GetUserCountAsync(),
                    totalOrders = await _orderService.GetOrderCountAsync(),
                    totalRevenue = await _orderService.GetTotalRevenueAsync(),
                    pendingOrders = await _orderService.GetPendingOrderCountAsync(),
                    lowStockProducts = await _productService.GetLowStockProductCountAsync()
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }

    public class AdminLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateUserStatusRequest
    {
        public bool IsActive { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
    }
}

