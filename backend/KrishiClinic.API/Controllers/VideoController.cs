using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KrishiClinic.API.Services;
using KrishiClinic.API.DTOs;
using KrishiClinic.API.Data;
using System.Security.Claims;

namespace KrishiClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly KrishiClinicDbContext _context;

        public VideoController(IVideoService videoService, KrishiClinicDbContext context)
        {
            _videoService = videoService;
            _context = context;
        }

        // Public endpoint for mobile app to get active videos
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<object>>> GetActiveVideos()
        {
            try
            {
                var videos = await _videoService.GetActiveVideosAsync();
                var videoDtos = videos.Select(v => new
                {
                    videoId = v.VideoId,
                    title = v.Title,
                    description = v.Description,
                    videoUrl = v.VideoUrl,
                    thumbnailUrl = v.ThumbnailUrl,
                    category = v.Category,
                    duration = v.Duration,
                    displayOrder = v.DisplayOrder,
                    createdAt = v.CreatedAt
                });

                return Ok(videoDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetActiveVideos: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        // Admin endpoints
        [HttpGet("admin/all")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetAllVideos()
        {
            try
            {
                var videos = await _videoService.GetAllVideosAsync();
                var videoDtos = videos.Select(v => new
                {
                    videoId = v.VideoId,
                    title = v.Title,
                    description = v.Description,
                    videoUrl = v.VideoUrl,
                    thumbnailUrl = v.ThumbnailUrl,
                    category = v.Category,
                    duration = v.Duration,
                    isActive = v.IsActive,
                    displayOrder = v.DisplayOrder,
                    createdAt = v.CreatedAt,
                    updatedAt = v.UpdatedAt,
                    createdBy = v.CreatedByAdmin != null ? new
                    {
                        adminId = v.CreatedByAdmin.AdminId,
                        name = v.CreatedByAdmin.Name,
                        email = v.CreatedByAdmin.Email
                    } : null
                });

                return Ok(videoDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllVideos: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("admin/{id}")]
        [Authorize]
        public async Task<ActionResult<object>> GetVideo(int id)
        {
            try
            {
                var video = await _videoService.GetVideoByIdAsync(id);
                if (video == null)
                    return NotFound(new { message = "Video not found" });

                var videoDto = new
                {
                    videoId = video.VideoId,
                    title = video.Title,
                    description = video.Description,
                    videoUrl = video.VideoUrl,
                    thumbnailUrl = video.ThumbnailUrl,
                    category = video.Category,
                    duration = video.Duration,
                    isActive = video.IsActive,
                    displayOrder = video.DisplayOrder,
                    createdAt = video.CreatedAt,
                    updatedAt = video.UpdatedAt,
                    createdBy = video.CreatedByAdmin != null ? new
                    {
                        adminId = video.CreatedByAdmin.AdminId,
                        name = video.CreatedByAdmin.Name,
                        email = video.CreatedByAdmin.Email
                    } : null
                };

                return Ok(videoDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetVideo: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpPost("admin")]
        [Authorize]
        public async Task<ActionResult<object>> CreateVideo([FromBody] CreateVideoDto videoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var adminId = GetAdminId();
                var video = await _videoService.CreateVideoAsync(videoDto, adminId);
                
                var videoResponse = new
                {
                    videoId = video.VideoId,
                    title = video.Title,
                    description = video.Description,
                    videoUrl = video.VideoUrl,
                    thumbnailUrl = video.ThumbnailUrl,
                    category = video.Category,
                    duration = video.Duration,
                    isActive = video.IsActive,
                    displayOrder = video.DisplayOrder,
                    createdAt = video.CreatedAt,
                    message = "Video created successfully"
                };

                return CreatedAtAction(nameof(GetVideo), new { id = video.VideoId }, videoResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateVideo: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpPut("admin/{id}")]
        [Authorize]
        public async Task<ActionResult<object>> UpdateVideo(int id, [FromBody] UpdateVideoDto videoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var video = await _videoService.UpdateVideoAsync(id, videoDto);
                
                var videoResponse = new
                {
                    videoId = video.VideoId,
                    title = video.Title,
                    description = video.Description,
                    videoUrl = video.VideoUrl,
                    thumbnailUrl = video.ThumbnailUrl,
                    category = video.Category,
                    duration = video.Duration,
                    isActive = video.IsActive,
                    displayOrder = video.DisplayOrder,
                    createdAt = video.CreatedAt,
                    updatedAt = video.UpdatedAt,
                    message = "Video updated successfully"
                };

                return Ok(videoResponse);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateVideo: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpDelete("admin/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteVideo(int id)
        {
            try
            {
                var result = await _videoService.DeleteVideoAsync(id);
                if (!result)
                    return NotFound(new { message = "Video not found" });

                return Ok(new { message = "Video deleted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteVideo: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpPut("admin/{id}/toggle-status")]
        [Authorize]
        public async Task<ActionResult<object>> ToggleVideoStatus(int id)
        {
            try
            {
                var result = await _videoService.ToggleVideoStatusAsync(id);
                if (!result)
                    return NotFound(new { message = "Video not found" });

                return Ok(new { message = "Video status updated successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ToggleVideoStatus: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("admin/stats")]
        [Authorize]
        public async Task<ActionResult<object>> GetVideoStats()
        {
            try
            {
                var totalVideos = await _videoService.GetVideoCountAsync();
                var activeVideos = await _context.Videos.CountAsync(v => v.IsActive);
                var inactiveVideos = totalVideos - activeVideos;

                var stats = new
                {
                    totalVideos = totalVideos,
                    activeVideos = activeVideos,
                    inactiveVideos = inactiveVideos
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetVideoStats: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        private int GetAdminId()
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(adminIdClaim ?? "0");
        }
    }
}
