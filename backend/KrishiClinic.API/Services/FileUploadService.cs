using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace KrishiClinic.API.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "images");
        Task<string> UploadVideoAsync(IFormFile file, string folder = "videos");
        Task<string> UploadThumbnailAsync(IFormFile file, string folder = "thumbnails");
        Task<bool> DeleteImageAsync(string imagePath);
        Task<bool> DeleteVideoAsync(string videoPath);
        Task<bool> DeleteThumbnailAsync(string thumbnailPath);
        Task<string[]> UploadMultipleImagesAsync(IFormFile[] files, string folder = "images");
        Task<string[]> UploadMultipleVideosAsync(IFormFile[] files, string folder = "videos");
    }

    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadPath;
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly string[] _allowedVideoExtensions = { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".webm" };
        private const long MaxImageSize = 5 * 1024 * 1024; // 5MB
        private const long MaxVideoSize = 100 * 1024 * 1024; // 100MB

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
            
            // Create upload directory if it doesn't exist
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder = "images")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided");

            if (file.Length > MaxImageSize)
                throw new ArgumentException($"File size exceeds {MaxImageSize / (1024 * 1024)}MB limit");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedImageExtensions.Contains(extension))
                throw new ArgumentException($"File type {extension} is not allowed");

            // Create folder if it doesn't exist
            var folderPath = Path.Combine(_uploadPath, folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative URL path
            return $"/uploads/{folder}/{fileName}";
        }

        public async Task<string[]> UploadMultipleImagesAsync(IFormFile[] files, string folder = "images")
        {
            if (files == null || files.Length == 0)
                throw new ArgumentException("No files provided");

            var uploadedPaths = new List<string>();

            foreach (var file in files)
            {
                try
                {
                    var path = await UploadImageAsync(file, folder);
                    uploadedPaths.Add(path);
                }
                catch (Exception ex)
                {
                    // Log error but continue with other files
                    Console.WriteLine($"Error uploading file {file.FileName}: {ex.Message}");
                }
            }

            return uploadedPaths.ToArray();
        }

        public async Task<string> UploadVideoAsync(IFormFile file, string folder = "videos")
        {
            try
            {
                Console.WriteLine($"UploadVideoAsync called with folder: {folder}");
                Console.WriteLine($"Upload path: {_uploadPath}");
                
                if (file == null || file.Length == 0)
                {
                    Console.WriteLine("No file provided");
                    throw new ArgumentException("No file provided");
                }

                Console.WriteLine($"File: {file.FileName}, Size: {file.Length}");

                if (file.Length > MaxVideoSize)
                {
                    Console.WriteLine($"File size exceeds limit: {file.Length} > {MaxVideoSize}");
                    throw new ArgumentException($"File size exceeds {MaxVideoSize / (1024 * 1024)}MB limit");
                }

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                Console.WriteLine($"File extension: {fileExtension}");
                
                if (!_allowedVideoExtensions.Contains(fileExtension))
                {
                    Console.WriteLine($"Invalid video file type: {fileExtension}");
                    throw new ArgumentException($"Invalid video file type. Allowed: {string.Join(", ", _allowedVideoExtensions)}");
                }

                var folderPath = Path.Combine(_uploadPath, folder);
                Console.WriteLine($"Folder path: {folderPath}");
                
                if (!Directory.Exists(folderPath))
                {
                    Console.WriteLine($"Creating directory: {folderPath}");
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(folderPath, fileName);
                Console.WriteLine($"File path: {filePath}");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var result = $"/uploads/{folder}/{fileName}";
                Console.WriteLine($"Upload successful: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UploadVideoAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<string> UploadThumbnailAsync(IFormFile file, string folder = "thumbnails")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided");

            if (file.Length > MaxImageSize)
                throw new ArgumentException($"File size exceeds {MaxImageSize / (1024 * 1024)}MB limit");

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedImageExtensions.Contains(fileExtension))
                throw new ArgumentException($"Invalid image file type. Allowed: {string.Join(", ", _allowedImageExtensions)}");

            var folderPath = Path.Combine(_uploadPath, folder);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folder}/{fileName}";
        }

        public async Task<string[]> UploadMultipleVideosAsync(IFormFile[] files, string folder = "videos")
        {
            var uploadedPaths = new List<string>();

            foreach (var file in files)
            {
                try
                {
                    var path = await UploadVideoAsync(file, folder);
                    uploadedPaths.Add(path);
                }
                catch (Exception ex)
                {
                    // Log error but continue with other files
                    Console.WriteLine($"Error uploading video {file.FileName}: {ex.Message}");
                }
            }

            return uploadedPaths.ToArray();
        }

        public async Task<bool> DeleteVideoAsync(string videoPath)
        {
            try
            {
                if (string.IsNullOrEmpty(videoPath))
                    return false;

                var fullPath = Path.Combine(_uploadPath, videoPath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting video: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteThumbnailAsync(string thumbnailPath)
        {
            try
            {
                if (string.IsNullOrEmpty(thumbnailPath))
                    return false;

                var fullPath = Path.Combine(_uploadPath, thumbnailPath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting thumbnail: {ex.Message}");
                return false;
            }
        }

        public Task<bool> DeleteImageAsync(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                    return Task.FromResult(false);

                // Remove leading slash and convert to full path
                var relativePath = imagePath.TrimStart('/');
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image {imagePath}: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}
