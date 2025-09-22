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

        [HttpPost("admin/login")]
        public async Task<ActionResult<object>> AdminLogin([FromBody] AdminLoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var isValid = await _adminService.VerifyAdminPasswordAsync(loginDto.Email, loginDto.Password);
                if (!isValid)
                    return Unauthorized(new { message = "Invalid credentials" });

                var admin = await _adminService.GetAdminByEmailAsync(loginDto.Email);
                if (admin == null)
                    return NotFound(new { message = "Admin not found" });

                var token = await _adminService.GenerateJwtTokenAsync(admin);
                return Ok(new { token = token, admin = admin });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
