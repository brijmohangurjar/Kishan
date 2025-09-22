using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;

namespace KrishiClinic.API.Services
{
    public interface IAdminService
    {
        Task<Admin?> GetAdminByEmailAsync(string email);
        Task<Admin> CreateAdminAsync(CreateAdminDto adminDto);
        Task<bool> VerifyAdminPasswordAsync(string email, string password);
        Task<string> GenerateJwtTokenAsync(Admin admin);
        Task<IEnumerable<Admin>> GetAllAdminsAsync();
        Task<bool> DeleteAdminAsync(int adminId);
        Task<object?> LoginAsync(string email, string password);
    }
}
