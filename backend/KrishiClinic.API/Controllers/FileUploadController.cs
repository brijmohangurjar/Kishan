using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KrishiClinic.API.Services;

namespace KrishiClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;

        public FileUploadController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [HttpPost("upload-image")]
        [Authorize]
        public async Task<ActionResult<object>> UploadImage(IFormFile file, string folder = "images")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file provided" });
                }

                var imageUrl = await _fileUploadService.UploadImageAsync(file, folder);
                
                return Ok(new
                {
                    success = true,
                    imageUrl = imageUrl,
                    fileName = file.FileName,
                    fileSize = file.Length,
                    message = "Image uploaded successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading image: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpPost("upload-multiple-images")]
        [Authorize]
        public async Task<ActionResult<object>> UploadMultipleImages(IFormFile[] files, string folder = "images")
        {
            try
            {
                if (files == null || files.Length == 0)
                {
                    return BadRequest(new { message = "No files provided" });
                }

                if (files.Length > 10)
                {
                    return BadRequest(new { message = "Maximum 10 files allowed" });
                }

                var imageUrls = await _fileUploadService.UploadMultipleImagesAsync(files, folder);
                
                return Ok(new
                {
                    success = true,
                    imageUrls = imageUrls,
                    uploadedCount = imageUrls.Length,
                    totalFiles = files.Length,
                    message = $"{imageUrls.Length} images uploaded successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading images: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpDelete("delete-image")]
        [Authorize]
        public async Task<ActionResult<object>> DeleteImage(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    return BadRequest(new { message = "Image path is required" });
                }

                var result = await _fileUploadService.DeleteImageAsync(imagePath);
                
                if (result)
                {
                    return Ok(new { success = true, message = "Image deleted successfully" });
                }
                else
                {
                    return NotFound(new { message = "Image not found or could not be deleted" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("test")]
        public ActionResult<object> TestEndpoint()
        {
            return Ok(new { message = "FileUpload controller is working", timestamp = DateTime.UtcNow });
        }

        [HttpPost("simple-video-upload")]
        public async Task<ActionResult<object>> SimpleVideoUpload(IFormFile file)
        {
            try
            {
                Console.WriteLine("SimpleVideoUpload endpoint called");
                
                if (file == null || file.Length == 0)
                {
                    Console.WriteLine("No file provided");
                    return BadRequest(new { message = "No file provided" });
                }

                Console.WriteLine($"File received: {file.FileName}, Size: {file.Length}");
                
                // Simple file save without FileUploadService
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }
                
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
                var videoUrl = $"/uploads/videos/{fileName}";
                var fullVideoUrl = $"http://localhost:5228{videoUrl}";
                Console.WriteLine($"Simple upload successful: {fullVideoUrl}");
                
                return Ok(new
                {
                    success = true,
                    videoUrl = fullVideoUrl,
                    message = "Simple video upload successful"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in SimpleVideoUpload: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Error in simple upload", details = ex.Message });
            }
        }

        [HttpPost("test-video-upload")]
        public async Task<ActionResult<object>> TestVideoUpload(IFormFile file)
        {
            try
            {
                Console.WriteLine("TestVideoUpload endpoint called");
                
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file provided" });
                }

                Console.WriteLine($"Test file received: {file.FileName}, Size: {file.Length}");
                
                return Ok(new
                {
                    success = true,
                    message = "Test video upload endpoint is working",
                    fileName = file.FileName,
                    fileSize = file.Length
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in TestVideoUpload: {ex.Message}");
                return StatusCode(500, new { message = "Error in test endpoint", details = ex.Message });
            }
        }

        [HttpPost("upload-video")]
        [Authorize]
        public async Task<ActionResult<object>> UploadVideo(IFormFile file, string folder = "videos")
        {
            try
            {
                Console.WriteLine($"UploadVideo endpoint called with folder: {folder}");
                
                if (file == null || file.Length == 0)
                {
                    Console.WriteLine("No file provided");
                    return BadRequest(new { message = "No file provided" });
                }

                Console.WriteLine($"File received: {file.FileName}, Size: {file.Length}");
                
                // Use simple file save approach (same as simple-video-upload)
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folder);
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }
                
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
                var videoUrl = $"/uploads/{folder}/{fileName}";
                var fullVideoUrl = $"http://localhost:5228{videoUrl}";
                Console.WriteLine($"Video uploaded successfully: {fullVideoUrl}");
                
                return Ok(new
                {
                    success = true,
                    videoUrl = fullVideoUrl,
                    message = "Video uploaded successfully"
                });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException in UploadVideo: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in UploadVideo: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Error uploading video", details = ex.Message });
            }
        }

        [HttpPost("upload-thumbnail")]
        [Authorize]
        public async Task<ActionResult<object>> UploadThumbnail(IFormFile file, string folder = "thumbnails")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file provided" });
                }

                var thumbnailUrl = await _fileUploadService.UploadThumbnailAsync(file, folder);
                
                return Ok(new
                {
                    success = true,
                    thumbnailUrl = thumbnailUrl,
                    message = "Thumbnail uploaded successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading thumbnail", details = ex.Message });
            }
        }

        [HttpPost("upload-multiple-videos")]
        [Authorize]
        public async Task<ActionResult<object>> UploadMultipleVideos(IFormFile[] files, string folder = "videos")
        {
            try
            {
                if (files == null || files.Length == 0)
                {
                    return BadRequest(new { message = "No files provided" });
                }

                var videoUrls = await _fileUploadService.UploadMultipleVideosAsync(files, folder);
                
                return Ok(new
                {
                    success = true,
                    videoUrls = videoUrls,
                    message = $"{videoUrls.Length} videos uploaded successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading videos", details = ex.Message });
            }
        }

        [HttpDelete("delete-video")]
        [Authorize]
        public async Task<ActionResult<object>> DeleteVideo([FromQuery] string videoPath)
        {
            try
            {
                if (string.IsNullOrEmpty(videoPath))
                {
                    return BadRequest(new { message = "Video path is required" });
                }

                var result = await _fileUploadService.DeleteVideoAsync(videoPath);
                
                if (result)
                {
                    return Ok(new { success = true, message = "Video deleted successfully" });
                }
                else
                {
                    return NotFound(new { message = "Video not found or could not be deleted" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting video", details = ex.Message });
            }
        }

        [HttpDelete("delete-thumbnail")]
        [Authorize]
        public async Task<ActionResult<object>> DeleteThumbnail([FromQuery] string thumbnailPath)
        {
            try
            {
                if (string.IsNullOrEmpty(thumbnailPath))
                {
                    return BadRequest(new { message = "Thumbnail path is required" });
                }

                var result = await _fileUploadService.DeleteThumbnailAsync(thumbnailPath);
                
                if (result)
                {
                    return Ok(new { success = true, message = "Thumbnail deleted successfully" });
                }
                else
                {
                    return NotFound(new { message = "Thumbnail not found or could not be deleted" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting thumbnail", details = ex.Message });
            }
        }

        [HttpGet("image-info")]
        public ActionResult<object> GetImageInfo()
        {
            return Ok(new
            {
                allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" },
                maxFileSize = "5MB",
                maxFilesPerUpload = 10,
                uploadPath = "/uploads/"
            });
        }
    }
}
