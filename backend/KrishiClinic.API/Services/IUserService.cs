using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;

namespace KrishiClinic.API.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByMobileAsync(string mobile);
        Task<User> CreateUserAsync(CreateUserDto userDto);
        Task<User> UpdateUserAsync(int userId, UpdateUserDto userDto);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> VerifyOtpAsync(string mobile, string otp);
        Task<string> GenerateOtpAsync(string mobile);
        Task<string> GenerateJwtTokenAsync(User user);
        Task<User?> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> UpdateUserStatusAsync(int userId, bool isActive);
        Task<int> GetUserCountAsync();
    }
}
