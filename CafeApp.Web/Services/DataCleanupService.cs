using CafeApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CafeApp.Web.Services
{
    public interface IDataCleanupService
    {
        Task CleanupOrphanedImageReferencesAsync();
        Task AssignSampleImagesToProductsAsync();
    }

    public class DataCleanupService : IDataCleanupService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<DataCleanupService> _logger;

        public DataCleanupService(
            ApplicationDbContext context, 
            IWebHostEnvironment webHostEnvironment,
            ILogger<DataCleanupService> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task CleanupOrphanedImageReferencesAsync()
        {
            _logger.LogInformation("Starting cleanup of orphaned image references...");

            var productsWithImages = await _context.Products
                .Where(p => !string.IsNullOrEmpty(p.ImageUrl))
                .ToListAsync();

            int cleanedCount = 0;

            foreach (var product in productsWithImages)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    // Check if the image file exists
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, 
                        product.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    if (!File.Exists(imagePath))
                    {
                        _logger.LogInformation("Removing orphaned image reference for product {ProductName}: {ImageUrl}", 
                            product.Name, product.ImageUrl);
                        
                        product.ImageUrl = null;
                        product.UpdatedAt = DateTime.UtcNow;
                        cleanedCount++;
                    }
                }
            }

            if (cleanedCount > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cleaned up {Count} orphaned image references", cleanedCount);
            }
            else
            {
                _logger.LogInformation("No orphaned image references found");
            }
        }

        public async Task AssignSampleImagesToProductsAsync()
        {
            _logger.LogInformation("Assigning sample images to products without images...");

            // Create sample images directory if it doesn't exist
            var sampleImagesPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "sample");
            if (!Directory.Exists(sampleImagesPath))
            {
                Directory.CreateDirectory(sampleImagesPath);
                _logger.LogInformation("Created sample images directory: {Path}", sampleImagesPath);
            }

            // Create placeholder images for products that don't have images
            await CreatePlaceholderImagesAsync();

            // Assign sample images to products
            var productsWithoutImages = await _context.Products
                .Where(p => string.IsNullOrEmpty(p.ImageUrl))
                .Include(p => p.Category)
                .ToListAsync();

            var sampleImageMappings = GetSampleImageMappings();
            int assignedCount = 0;

            foreach (var product in productsWithoutImages)
            {
                if (sampleImageMappings.TryGetValue(product.Name.ToLower(), out string? imageUrl))
                {
                    product.ImageUrl = imageUrl;
                    product.UpdatedAt = DateTime.UtcNow;
                    assignedCount++;
                    _logger.LogInformation("Assigned sample image to {ProductName}: {ImageUrl}", 
                        product.Name, imageUrl);
                }
                else
                {
                    // Assign category-based placeholder
                    var categoryPlaceholder = GetCategoryPlaceholder(product.Category?.Name);
                    if (!string.IsNullOrEmpty(categoryPlaceholder))
                    {
                        product.ImageUrl = categoryPlaceholder;
                        product.UpdatedAt = DateTime.UtcNow;
                        assignedCount++;
                        _logger.LogInformation("Assigned category placeholder to {ProductName}: {ImageUrl}", 
                            product.Name, categoryPlaceholder);
                    }
                }
            }

            if (assignedCount > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Assigned sample images to {Count} products", assignedCount);
            }
            else
            {
                _logger.LogInformation("All products already have images assigned");
            }
        }

        private async Task CreatePlaceholderImagesAsync()
        {
            var sampleImagesPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "sample");
            
            // Create simple SVG placeholders for different categories
            var placeholders = new Dictionary<string, string>
            {
                ["coffee-placeholder.svg"] = CreateCoffeePlaceholderSvg(),
                ["tea-placeholder.svg"] = CreateTeaPlaceholderSvg(),
                ["pastry-placeholder.svg"] = CreatePastryPlaceholderSvg(),
                ["sandwich-placeholder.svg"] = CreateSandwichPlaceholderSvg(),
                ["dessert-placeholder.svg"] = CreateDessertPlaceholderSvg(),
                ["smoothie-placeholder.svg"] = CreateSmoothiePlaceholderSvg()
            };

            foreach (var placeholder in placeholders)
            {
                var filePath = Path.Combine(sampleImagesPath, placeholder.Key);
                if (!File.Exists(filePath))
                {
                    await File.WriteAllTextAsync(filePath, placeholder.Value);
                    _logger.LogInformation("Created placeholder image: {FileName}", placeholder.Key);
                }
            }
        }

        private Dictionary<string, string> GetSampleImageMappings()
        {
            return new Dictionary<string, string>
            {
                // Coffee
                ["espresso"] = "/images/sample/coffee-placeholder.svg",
                ["americano"] = "/images/sample/coffee-placeholder.svg",
                ["cappuccino"] = "/images/sample/coffee-placeholder.svg",
                ["latte"] = "/images/sample/coffee-placeholder.svg",
                ["mocha"] = "/images/sample/coffee-placeholder.svg",
                ["iced coffee"] = "/images/sample/coffee-placeholder.svg",
                
                // Tea
                ["green tea"] = "/images/sample/tea-placeholder.svg",
                ["earl grey"] = "/images/sample/tea-placeholder.svg",
                ["chamomile"] = "/images/sample/tea-placeholder.svg",
                ["chai latte"] = "/images/sample/tea-placeholder.svg",
                
                // Pastries
                ["croissant"] = "/images/sample/pastry-placeholder.svg",
                ["blueberry muffin"] = "/images/sample/pastry-placeholder.svg",
                ["danish"] = "/images/sample/pastry-placeholder.svg",
                ["bagel"] = "/images/sample/pastry-placeholder.svg",
                
                // Sandwiches
                ["club sandwich"] = "/images/sample/sandwich-placeholder.svg",
                ["grilled cheese"] = "/images/sample/sandwich-placeholder.svg",
                ["blt"] = "/images/sample/sandwich-placeholder.svg",
                ["veggie wrap"] = "/images/sample/sandwich-placeholder.svg",
                
                // Desserts
                ["chocolate cake"] = "/images/sample/dessert-placeholder.svg",
                ["cheesecake"] = "/images/sample/dessert-placeholder.svg",
                ["tiramisu"] = "/images/sample/dessert-placeholder.svg",
                ["cookies"] = "/images/sample/dessert-placeholder.svg",
                
                // Smoothies
                ["berry smoothie"] = "/images/sample/smoothie-placeholder.svg",
                ["mango smoothie"] = "/images/sample/smoothie-placeholder.svg",
                ["green smoothie"] = "/images/sample/smoothie-placeholder.svg",
                ["protein smoothie"] = "/images/sample/smoothie-placeholder.svg"
            };
        }

        private string GetCategoryPlaceholder(string? categoryName)
        {
            return categoryName?.ToLower() switch
            {
                "coffee" => "/images/sample/coffee-placeholder.svg",
                "tea" => "/images/sample/tea-placeholder.svg",
                "pastries" => "/images/sample/pastry-placeholder.svg",
                "sandwiches" => "/images/sample/sandwich-placeholder.svg",
                "desserts" => "/images/sample/dessert-placeholder.svg",
                "smoothies" => "/images/sample/smoothie-placeholder.svg",
                _ => "/images/placeholder-product.svg"
            };
        }

        private string CreateCoffeePlaceholderSvg()
        {
            return @"<svg width=""400"" height=""300"" xmlns=""http://www.w3.org/2000/svg"">
  <rect width=""100%"" height=""100%"" fill=""#8B4513""/>
  <circle cx=""200"" cy=""150"" r=""80"" fill=""#D2691E"" stroke=""#654321"" stroke-width=""3""/>
  <circle cx=""200"" cy=""150"" r=""60"" fill=""#F4A460""/>
  <text x=""200"" y=""250"" font-family=""Arial, sans-serif"" font-size=""18"" fill=""white"" text-anchor=""middle"">Coffee</text>
</svg>";
        }

        private string CreateTeaPlaceholderSvg()
        {
            return @"<svg width=""400"" height=""300"" xmlns=""http://www.w3.org/2000/svg"">
  <rect width=""100%"" height=""100%"" fill=""#228B22""/>
  <circle cx=""200"" cy=""150"" r=""80"" fill=""#90EE90"" stroke=""#006400"" stroke-width=""3""/>
  <circle cx=""200"" cy=""150"" r=""60"" fill=""#98FB98""/>
  <text x=""200"" y=""250"" font-family=""Arial, sans-serif"" font-size=""18"" fill=""white"" text-anchor=""middle"">Tea</text>
</svg>";
        }

        private string CreatePastryPlaceholderSvg()
        {
            return @"<svg width=""400"" height=""300"" xmlns=""http://www.w3.org/2000/svg"">
  <rect width=""100%"" height=""100%"" fill=""#DEB887""/>
  <circle cx=""200"" cy=""150"" r=""80"" fill=""#F5DEB3"" stroke=""#D2691E"" stroke-width=""3""/>
  <circle cx=""200"" cy=""150"" r=""60"" fill=""#FFEBCD""/>
  <text x=""200"" y=""250"" font-family=""Arial, sans-serif"" font-size=""18"" fill=""#8B4513"" text-anchor=""middle"">Pastry</text>
</svg>";
        }

        private string CreateSandwichPlaceholderSvg()
        {
            return @"<svg width=""400"" height=""300"" xmlns=""http://www.w3.org/2000/svg"">
  <rect width=""100%"" height=""100%"" fill=""#CD853F""/>
  <rect x=""120"" y=""120"" width=""160"" height=""60"" fill=""#F5DEB3"" stroke=""#8B4513"" stroke-width=""3""/>
  <rect x=""130"" y=""130"" width=""140"" height=""40"" fill=""#FFE4B5""/>
  <text x=""200"" y=""250"" font-family=""Arial, sans-serif"" font-size=""18"" fill=""white"" text-anchor=""middle"">Sandwich</text>
</svg>";
        }

        private string CreateDessertPlaceholderSvg()
        {
            return @"<svg width=""400"" height=""300"" xmlns=""http://www.w3.org/2000/svg"">
  <rect width=""100%"" height=""100%"" fill=""#FF69B4""/>
  <circle cx=""200"" cy=""150"" r=""80"" fill=""#FFB6C1"" stroke=""#DC143C"" stroke-width=""3""/>
  <circle cx=""200"" cy=""150"" r=""60"" fill=""#FFC0CB""/>
  <text x=""200"" y=""250"" font-family=""Arial, sans-serif"" font-size=""18"" fill=""white"" text-anchor=""middle"">Dessert</text>
</svg>";
        }

        private string CreateSmoothiePlaceholderSvg()
        {
            return @"<svg width=""400"" height=""300"" xmlns=""http://www.w3.org/2000/svg"">
  <rect width=""100%"" height=""100%"" fill=""#9370DB""/>
  <circle cx=""200"" cy=""150"" r=""80"" fill=""#DDA0DD"" stroke=""#8A2BE2"" stroke-width=""3""/>
  <circle cx=""200"" cy=""150"" r=""60"" fill=""#E6E6FA""/>
  <text x=""200"" y=""250"" font-family=""Arial, sans-serif"" font-size=""18"" fill=""white"" text-anchor=""middle"">Smoothie</text>
</svg>";
        }
    }
}
