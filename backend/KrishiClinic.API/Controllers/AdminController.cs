using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KrishiClinic.API.Services;
using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;
using KrishiClinic.API.Data;
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
        private readonly KrishiClinicDbContext _context;

        public AdminController(
            IAdminService adminService,
            IProductService productService,
            IUserService userService,
            IOrderService orderService,
            KrishiClinicDbContext context)
        {
            _adminService = adminService;
            _productService = productService;
            _userService = userService;
            _orderService = orderService;
            _context = context;
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

        [HttpPost("users")]
        [Authorize]
        public async Task<ActionResult<object>> CreateUser([FromBody] CreateUserDto userDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userService.GetUserByMobileAsync(userDto.Mobile);
                if (existingUser != null)
                    return Conflict(new { message = "User with this mobile number already exists" });

                var user = await _userService.CreateUserAsync(userDto);
                
                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, new { 
                    message = "User created successfully",
                    user = new {
                        userId = user.UserId,
                        name = user.Name,
                        mobile = user.Mobile,
                        village = user.Village,
                        isActive = user.IsActive,
                        createdAt = user.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("users/{id}")]
        [Authorize]
        public async Task<ActionResult<object>> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, userDto);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(new { 
                    message = "User updated successfully",
                    user = new {
                        userId = user.UserId,
                        name = user.Name,
                        mobile = user.Mobile,
                        village = user.Village,
                        isActive = user.IsActive,
                        updatedAt = user.UpdatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("users/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var success = await _userService.DeleteUserAsync(id);
                if (!success)
                    return NotFound(new { message = "User not found" });

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("db-check")]
        [Authorize]
        public async Task<ActionResult<object>> DatabaseCheck()
        {
            try
            {
                // Check if we can connect to the database at all
                var canConnect = await _context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    return StatusCode(500, new { message = "Cannot connect to database" });
                }

                return Ok(new { 
                    message = "Database connection successful",
                    canConnect = canConnect,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DatabaseCheck: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Database check failed", details = ex.Message });
            }
        }

        [HttpGet("simple-test")]
        [Authorize]
        public ActionResult<object> SimpleTest()
        {
            return Ok(new { message = "Admin controller is working", timestamp = DateTime.UtcNow });
        }

        [HttpGet("orders-simple")]
        [Authorize]
        public async Task<ActionResult<object>> GetOrdersSimple()
        {
            try
            {
                // Try to get just the count first
                var orderCount = await _orderService.GetOrderCountAsync();
                return Ok(new { 
                    message = "Orders query successful", 
                    count = orderCount,
                    timestamp = DateTime.UtcNow 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOrdersSimple: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Orders query failed", details = ex.Message });
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
                // Log the full exception details
                Console.WriteLine($"Error in GetOrders: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
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
                var success = await _orderService.UpdateOrderStatusAsync(id, request.Status, request.GetShippedDate(), request.GetDeliveredDate());
                if (!success)
                    return NotFound(new { message = "Order not found" });

                return Ok(new { message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("stats")]
        [Authorize]
        public async Task<ActionResult<object>> GetAdminStats()
        {
            try
            {
                var stats = new
                {
                    totalOrders = await _orderService.GetOrderCountAsync(),
                    pendingOrders = await _orderService.GetOrderCountByStatusAsync("Pending"),
                    confirmedOrders = await _orderService.GetOrderCountByStatusAsync("Confirmed"),
                    shippedOrders = await _orderService.GetOrderCountByStatusAsync("Shipped"),
                    deliveredOrders = await _orderService.GetOrderCountByStatusAsync("Delivered"),
                    cancelledOrders = await _orderService.GetOrderCountByStatusAsync("Cancelled"),
                    totalRevenue = await _orderService.GetTotalRevenueAsync(),
                    recentOrders = await _orderService.GetRecentOrdersAsync(5)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                // Log the full exception details
                Console.WriteLine($"Error in GetAdminStats: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("test-db")]
        [Authorize]
        public async Task<ActionResult<object>> TestDatabase()
        {
            try
            {
                // Test basic database connection
                var userCount = await _userService.GetUserCountAsync();
                var productCount = await _productService.GetProductCountAsync();
                var orderCount = await _orderService.GetOrderCountAsync();
                
                return Ok(new { 
                    message = "Database connection successful",
                    userCount = userCount,
                    productCount = productCount,
                    orderCount = orderCount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TestDatabase: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Database connection failed", details = ex.Message });
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
        public string? ShippedDate { get; set; }
        public string? DeliveredDate { get; set; }
        
        public DateTime? GetShippedDate()
        {
            if (string.IsNullOrEmpty(ShippedDate))
                return null;
            return DateTime.TryParse(ShippedDate, out var date) ? date : null;
        }
        
        public DateTime? GetDeliveredDate()
        {
            if (string.IsNullOrEmpty(DeliveredDate))
                return null;
            return DateTime.TryParse(DeliveredDate, out var date) ? date : null;
        }
    }
}

