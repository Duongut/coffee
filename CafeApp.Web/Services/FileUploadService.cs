using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CafeApp.Web.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB

        public FileUploadService(IWebHostEnvironment webHostEnvironment, ILogger<FileUploadService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder = "products")
        {
            _logger.LogInformation("UploadImageAsync called with file: {FileName}, Size: {FileSize}",
                file?.FileName ?? "null", file?.Length ?? 0);

            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("File is null or empty");
                    throw new ArgumentException("No file provided");
                }

                if (!IsValidImageFile(file))
                {
                    _logger.LogWarning("Invalid image file: {FileName}, ContentType: {ContentType}, Size: {Size}",
                        file.FileName, file.ContentType, file.Length);
                    throw new ArgumentException("Invalid image file");
                }

                // Create upload directory if it doesn't exist
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", folder);
                _logger.LogInformation("Upload path: {UploadPath}", uploadPath);

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                    _logger.LogInformation("Created directory: {UploadPath}", uploadPath);
                }

                // Generate unique filename
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadPath, fileName);

                _logger.LogInformation("Saving file to: {FilePath}", filePath);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative URL
                var imageUrl = $"/images/{folder}/{fileName}";
                _logger.LogInformation("Image uploaded successfully: {ImageUrl}, File size: {FileSize} bytes",
                    imageUrl, file.Length);

                return imageUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image: {FileName}", file?.FileName ?? "unknown");
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl) || imageUrl.StartsWith("http"))
                {
                    return true; // External URL or empty, nothing to delete
                }

                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                    _logger.LogInformation("Image deleted successfully: {ImageUrl}", imageUrl);
                    return true;
                }

                return true; // File doesn't exist, consider it deleted
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image: {ImageUrl}", imageUrl);
                return false;
            }
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            // Check file size
            if (file.Length > _maxFileSize)
            {
                return false;
            }

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                return false;
            }

            // Check MIME type
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return false;
            }

            return true;
        }

        public string GetImagePath(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return string.Empty;
            }

            return Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
