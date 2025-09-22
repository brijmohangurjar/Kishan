using Microsoft.AspNetCore.Mvc;
using KrishiClinic.API.Services;
using KrishiClinic.API.DTOs;

namespace KrishiClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAdminService _adminService;

        public AuthController(IUserService userService, IAdminService adminService)
        {
            _userService = userService;
            _adminService = adminService;
        }

        [HttpPost("send-otp")]
        public async Task<ActionResult<object>> SendOtp([FromBody] OtpRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var otp = await _userService.GenerateOtpAsync(request.Mobile);
                return Ok(new { message = "OTP sent successfully", otp = otp }); // In production, don't return OTP
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<object>> VerifyOtp([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var isValid = await _userService.VerifyOtpAsync(loginDto.Mobile, loginDto.Otp);
                if (!isValid)
                    return Unauthorized(new { message = "Invalid OTP" });

                var user = await _userService.GetUserByMobileAsync(loginDto.Mobile);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                var token = await _userService.GenerateJwtTokenAsync(user);
                return Ok(new { token = token, user = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<object>> Register([FromBody] CreateUserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Check if user already exists
                var existingUser = await _userService.GetUserByMobileAsync(userDto.Mobile);
                if (existingUser != null)
                    return Conflict(new { message = "User with this mobile number already exists" });

                var user = await _userService.CreateUserAsync(userDto);
                var token = await _userService.GenerateJwtTokenAsync(user);
                
                return CreatedAtAction(nameof(GetUserProfile), new { id = user.UserId }, new { token = token, user = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("profile/{id}")]
        public async Task<ActionResult<object>> GetUserProfile(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("admin/check")]
        public async Task<ActionResult<object>> CheckAdmins()
        {
            try
            {
                var admins = await _adminService.GetAllAdminsAsync();
                return Ok(new { 
                    count = admins.Count(),
                    admins = admins.Select(a => new { 
                        id = a.AdminId, 
                        email = a.Email, 
                        name = a.Name, 
                        isActive = a.IsActive,
                        passwordHash = a.Password // Show the actual password hash
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("admin/test-login")]
        public async Task<ActionResult<object>> TestLogin([FromBody] AdminLoginDto loginDto)
        {
            var debugInfo = new List<string>();
            
            try
            {
                debugInfo.Add($"=== TEST LOGIN START ===");
                debugInfo.Add($"Email: {loginDto.Email}");
                debugInfo.Add($"Password: {loginDto.Password}");
                
                var admin = await _adminService.GetAdminByEmailAsync(loginDto.Email);
                debugInfo.Add($"Admin found: {admin != null}");
                
                if (admin != null)
                {
                    debugInfo.Add($"Admin ID: {admin.AdminId}");
                    debugInfo.Add($"Admin Name: {admin.Name}");
                    debugInfo.Add($"Admin Email: {admin.Email}");
                    debugInfo.Add($"Admin IsActive: {admin.IsActive}");
                    debugInfo.Add($"Admin Password Hash: {admin.Password}");
                    
                    var isValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, admin.Password);
                    debugInfo.Add($"Password verification: {isValid}");
                    
                    return Ok(new {
                        debugInfo = debugInfo,
                        adminFound = true,
                        adminId = admin.AdminId,
                        adminEmail = admin.Email,
                        adminName = admin.Name,
                        adminIsActive = admin.IsActive,
                        passwordHash = admin.Password,
                        passwordVerification = isValid,
                        result = isValid ? "SUCCESS" : "FAILED - Password verification failed"
                    });
                }
                else
                {
                    debugInfo.Add("Admin not found in database");
                    return Ok(new { 
                        debugInfo = debugInfo,
                        adminFound = false, 
                        message = "Admin not found",
                        result = "FAILED - Admin not found"
                    });
                }
            }
            catch (Exception ex)
            {
                debugInfo.Add($"Test login error: {ex.Message}");
                return BadRequest(new { 
                    debugInfo = debugInfo,
                    message = ex.Message, 
                    details = ex.ToString(),
                    result = "ERROR"
                });
            }
        }

        [HttpPost("admin/fix-password")]
        public async Task<ActionResult<object>> FixPassword([FromBody] AdminLoginDto loginDto)
        {
            try
            {
                // Generate correct hash
                var correctHash = BCrypt.Net.BCrypt.HashPassword(loginDto.Password);
                
                // Test the hash
                var isValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, correctHash);

                return Ok(new {
                    message = "Here's the correct password hash",
                    email = loginDto.Email,
                    password = loginDto.Password,
                    correctHash = correctHash,
                    verificationTest = isValid,
                    instruction = "Copy this hash and update your database manually"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, details = ex.ToString() });
            }
        }

        [HttpPost("admin/login")]
        public async Task<ActionResult<object>> AdminLogin([FromBody] AdminLoginDto loginDto)
        {
            var debugInfo = new List<string>();
            debugInfo.Add($"Login attempt for email: {loginDto.Email}");
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var admin = await _adminService.GetAdminByEmailAsync(loginDto.Email);
                debugInfo.Add($"Admin found: {admin != null}");
                
                if (admin != null)
                {
                    debugInfo.Add($"Admin ID: {admin.AdminId}");
                    debugInfo.Add($"Admin IsActive: {admin.IsActive}");
                    debugInfo.Add($"Admin Password Hash: {admin.Password}");
                }
                
                var isValid = await _adminService.VerifyAdminPasswordAsync(loginDto.Email, loginDto.Password);
                debugInfo.Add($"Password verification result: {isValid}");
                
                if (!isValid)
                    return Unauthorized(new { 
                        message = "Invalid credentials",
                        debugInfo = debugInfo,
                        result = "FAILED - Invalid credentials"
                    });

                if (admin == null)
                    return NotFound(new { 
                        message = "Admin not found",
                        debugInfo = debugInfo,
                        result = "FAILED - Admin not found"
                    });

                var token = await _adminService.GenerateJwtTokenAsync(admin);
                return Ok(new { 
                    token = token, 
                    admin = admin,
                    debugInfo = debugInfo,
                    result = "SUCCESS"
                });
            }
            catch (Exception ex)
            {
                debugInfo.Add($"Login error: {ex.Message}");
                return BadRequest(new { 
                    message = ex.Message,
                    debugInfo = debugInfo,
                    result = "ERROR"
                });
            }
        }
    }
}
