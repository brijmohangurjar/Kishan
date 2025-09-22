using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KrishiClinic.API.Data;
using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;

namespace KrishiClinic.API.Services
{
    public class UserService : IUserService
    {
        private readonly KrishiClinicDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IOtpService _otpService;

        public UserService(KrishiClinicDbContext context, IConfiguration configuration, IOtpService otpService)
        {
            _context = context;
            _configuration = configuration;
            _otpService = otpService;
        }

        public async Task<User?> GetUserByMobileAsync(string mobile)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Mobile == mobile);
        }

        public async Task<User> CreateUserAsync(CreateUserDto userDto)
        {
            var user = new User
            {
                Name = userDto.Name,
                Village = userDto.Village,
                Address = userDto.Address,
                Mobile = userDto.Mobile,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(int userId, UpdateUserDto userDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            if (!string.IsNullOrEmpty(userDto.Name))
                user.Name = userDto.Name;
            if (!string.IsNullOrEmpty(userDto.Village))
                user.Village = userDto.Village;
            if (!string.IsNullOrEmpty(userDto.Address))
                user.Address = userDto.Address;

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerifyOtpAsync(string mobile, string otp)
        {
            var user = await GetUserByMobileAsync(mobile);
            if (user == null || user.OTP != otp || user.OTPExpiry < DateTime.UtcNow)
                return false;

            // Clear OTP after successful verification
            user.OTP = null;
            user.OTPExpiry = null;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenerateOtpAsync(string mobile)
        {
            var user = await GetUserByMobileAsync(mobile);
            
            if (user == null)
            {
                throw new ArgumentException("User not found. Please register first.");
            }

            var otp = _otpService.GenerateOtp();
            user.OTP = otp;
            user.OTPExpiry = DateTime.UtcNow.AddMinutes(5); // OTP valid for 5 minutes
            await _context.SaveChangesAsync();

            // In a real application, send OTP via SMS
            await _otpService.SendOtpAsync(mobile, otp);
            
            return otp;
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.MobilePhone, user.Mobile),
                    new Claim("UserId", user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:ExpiryInHours"] ?? "24")),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
        }

        public async Task<bool> UpdateUserStatusAsync(int userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }
    }
}
