using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;

namespace KrishiClinic.API.Services
{
    public interface IVideoService
    {
        Task<IEnumerable<Video>> GetAllVideosAsync();
        Task<IEnumerable<Video>> GetActiveVideosAsync();
        Task<Video?> GetVideoByIdAsync(int videoId);
        Task<Video> CreateVideoAsync(CreateVideoDto videoDto, int createdBy);
        Task<Video> UpdateVideoAsync(int videoId, UpdateVideoDto videoDto);
        Task<bool> DeleteVideoAsync(int videoId);
        Task<bool> ToggleVideoStatusAsync(int videoId);
        Task<int> GetVideoCountAsync();
        Task<IEnumerable<Video>> GetVideosByCategoryAsync(string category);
    }
}
