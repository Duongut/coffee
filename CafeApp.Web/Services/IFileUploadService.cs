using Microsoft.AspNetCore.Http;

namespace CafeApp.Web.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "products");
        Task<bool> DeleteImageAsync(string imageUrl);
        bool IsValidImageFile(IFormFile file);
        string GetImagePath(string imageUrl);
    }

    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string? ImageUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
