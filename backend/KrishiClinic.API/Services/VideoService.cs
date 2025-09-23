using Microsoft.EntityFrameworkCore;
using KrishiClinic.API.Data;
using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;

namespace KrishiClinic.API.Services
{
    public class VideoService : IVideoService
    {
        private readonly KrishiClinicDbContext _context;

        public VideoService(KrishiClinicDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Video>> GetAllVideosAsync()
        {
            return await _context.Videos
                .Include(v => v.CreatedByAdmin)
                .OrderBy(v => v.DisplayOrder)
                .ThenByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Video>> GetActiveVideosAsync()
        {
            return await _context.Videos
                .Where(v => v.IsActive)
                .OrderBy(v => v.DisplayOrder)
                .ThenByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<Video?> GetVideoByIdAsync(int videoId)
        {
            return await _context.Videos
                .Include(v => v.CreatedByAdmin)
                .FirstOrDefaultAsync(v => v.VideoId == videoId);
        }

        public async Task<Video> CreateVideoAsync(CreateVideoDto videoDto, int createdBy)
        {
            var video = new Video
            {
                Title = videoDto.Title,
                Description = videoDto.Description,
                VideoUrl = videoDto.VideoUrl,
                ThumbnailUrl = videoDto.ThumbnailUrl,
                Category = videoDto.Category,
                Duration = videoDto.Duration,
                DisplayOrder = videoDto.DisplayOrder,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.Videos.Add(video);
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task<Video> UpdateVideoAsync(int videoId, UpdateVideoDto videoDto)
        {
            var video = await _context.Videos.FindAsync(videoId);
            if (video == null)
                throw new ArgumentException("Video not found");

            if (!string.IsNullOrEmpty(videoDto.Title))
                video.Title = videoDto.Title;

            if (videoDto.Description != null)
                video.Description = videoDto.Description;

            if (!string.IsNullOrEmpty(videoDto.VideoUrl))
                video.VideoUrl = videoDto.VideoUrl;

            if (videoDto.ThumbnailUrl != null)
                video.ThumbnailUrl = videoDto.ThumbnailUrl;

            if (!string.IsNullOrEmpty(videoDto.Category))
                video.Category = videoDto.Category;

            if (videoDto.Duration.HasValue)
                video.Duration = videoDto.Duration.Value;

            if (videoDto.DisplayOrder.HasValue)
                video.DisplayOrder = videoDto.DisplayOrder.Value;

            if (videoDto.IsActive.HasValue)
                video.IsActive = videoDto.IsActive.Value;

            video.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task<bool> DeleteVideoAsync(int videoId)
        {
            var video = await _context.Videos.FindAsync(videoId);
            if (video == null)
                return false;

            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleVideoStatusAsync(int videoId)
        {
            var video = await _context.Videos.FindAsync(videoId);
            if (video == null)
                return false;

            video.IsActive = !video.IsActive;
            video.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetVideoCountAsync()
        {
            return await _context.Videos.CountAsync();
        }

        public async Task<IEnumerable<Video>> GetVideosByCategoryAsync(string category)
        {
            return await _context.Videos
                .Where(v => v.Category == category && v.IsActive)
                .OrderBy(v => v.DisplayOrder)
                .ThenByDescending(v => v.CreatedAt)
                .ToListAsync();
        }
    }
}
