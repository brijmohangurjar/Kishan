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
    public class AdminService : IAdminService
    {
        private readonly KrishiClinicDbContext _context;
        private readonly IConfiguration _configuration;

        public AdminService(KrishiClinicDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Admin?> GetAdminByEmailAsync(string email)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Admin> CreateAdminAsync(CreateAdminDto adminDto)
        {
            Console.WriteLine($"Creating admin with email: {adminDto.Email}");
            Console.WriteLine($"Original password: {adminDto.Password}");
            
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(adminDto.Password);
            Console.WriteLine($"Hashed password: {hashedPassword}");
            
            var admin = new Admin
            {
                Name = adminDto.Name,
                Email = adminDto.Email,
                Password = hashedPassword,
                Role = adminDto.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"Admin created with ID: {admin.AdminId}");
            return admin;
        }

        public async Task<bool> VerifyAdminPasswordAsync(string email, string password)
        {
            var admin = await GetAdminByEmailAsync(email);
            Console.WriteLine($"Admin lookup for email: {email}");
            Console.WriteLine($"Admin found: {admin != null}");
            if (admin != null)
            {
                Console.WriteLine($"Admin IsActive: {admin.IsActive}");
                Console.WriteLine($"Admin Password Hash: {admin.Password}");
            }
            
            if (admin == null || !admin.IsActive)
                return false;

            var isValid = BCrypt.Net.BCrypt.Verify(password, admin.Password);
            Console.WriteLine($"Password verification result: {isValid}");
            return isValid;
        }

        public async Task<string> GenerateJwtTokenAsync(Admin admin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
                    new Claim(ClaimTypes.Name, admin.Name),
                    new Claim(ClaimTypes.Email, admin.Email),
                    new Claim(ClaimTypes.Role, admin.Role),
                    new Claim("AdminId", admin.AdminId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:ExpiryInHours"] ?? "24")),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            // Update last login time
            admin.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return tokenHandler.WriteToken(token);
        }

        public async Task<IEnumerable<Admin>> GetAllAdminsAsync()
        {
            return await _context.Admins.Where(a => a.IsActive).ToListAsync();
        }

        public async Task<bool> DeleteAdminAsync(int adminId)
        {
            var admin = await _context.Admins.FindAsync(adminId);
            if (admin == null)
                return false;

            admin.IsActive = false;
            admin.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<object?> LoginAsync(string email, string password)
        {
            var admin = await GetAdminByEmailAsync(email);
            if (admin == null || !admin.IsActive)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, admin.Password))
                return null;

            var token = await GenerateJwtTokenAsync(admin);
            
            return new
            {
                Token = token,
                Admin = new
                {
                    adminId = admin.AdminId,
                    name = admin.Name,
                    email = admin.Email,
                    isActive = admin.IsActive,
                    createdAt = admin.CreatedAt
                }
            };
        }
    }
}
