using CafeApp.Core.Entities;
using CafeApp.Core.Interfaces;
using CafeApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CafeApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ICategoryService categoryService, IFileUploadService fileUploadService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesDropDown();
            return View();
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,CategoryId,IsAvailable,IsFeatured,Size,Temperature")] Product product, IFormFile? ImageFile)
        {
            _logger.LogInformation("=== CREATE POST METHOD CALLED ===");
            _logger.LogInformation("Product: {ProductName}, Price: {Price}, CategoryId: {CategoryId}",
                product?.Name ?? "null", product?.Price ?? 0, product?.CategoryId ?? 0);
            _logger.LogInformation("ImageFile: {ImageFileName}, Size: {Size}",
                ImageFile?.FileName ?? "null", ImageFile?.Length ?? 0);

            // Log all form data received
            _logger.LogInformation("Form data received:");
            foreach (var key in Request.Form.Keys)
            {
                _logger.LogInformation("  {Key}: {Value}", key, Request.Form[key]);
            }

            // Log ModelState details
            _logger.LogInformation("ModelState.IsValid: {IsValid}", ModelState.IsValid);
            if (!ModelState.IsValid)
            {
                foreach (var modelError in ModelState)
                {
                    foreach (var error in modelError.Value.Errors)
                    {
                        _logger.LogWarning("ModelState Error - {Key}: {Error}", modelError.Key, error.ErrorMessage);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("ModelState is valid, processing product creation");

                    // Handle image upload
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        _logger.LogInformation("Image file provided: {FileName}, Size: {Size}, ContentType: {ContentType}",
                            ImageFile.FileName, ImageFile.Length, ImageFile.ContentType);

                        if (_fileUploadService.IsValidImageFile(ImageFile))
                        {
                            _logger.LogInformation("Image file is valid, uploading...");
                            product.ImageUrl = await _fileUploadService.UploadImageAsync(ImageFile, "products");
                            _logger.LogInformation("Image uploaded successfully: {ImageUrl}", product.ImageUrl);
                        }
                        else
                        {
                            _logger.LogWarning("Invalid image file: {FileName}", ImageFile.FileName);
                            ModelState.AddModelError("ImageFile", "Please select a valid image file (JPG, PNG, GIF, WEBP) under 5MB.");
                            await PopulateCategoriesDropDown(product.CategoryId);
                            return View(product);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("No image file provided");
                    }

                    _logger.LogInformation("Creating product in database...");
                    _logger.LogInformation("Product before save: Name={Name}, Price={Price}, CategoryId={CategoryId}, ImageUrl={ImageUrl}",
                        product.Name, product.Price, product.CategoryId, product.ImageUrl);

                    await _productService.CreateProductAsync(product);
                    _logger.LogInformation("Product created successfully: {ProductId}", product.Id);

                    TempData["SuccessMessage"] = "Product created successfully!";
                    _logger.LogInformation("Redirecting to Index with success message");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating product: {ProductName}", product?.Name);
                    ModelState.AddModelError("", "An error occurred while creating the product: " + ex.Message);
                }
            }

            _logger.LogInformation("Returning to Create view due to validation errors or exception");
            await PopulateCategoriesDropDown(product.CategoryId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            await PopulateCategoriesDropDown(product.CategoryId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,CategoryId,ImageUrl,IsAvailable,IsFeatured,Size,Temperature,CreatedAt")] Product product, IFormFile? ImageFile)
        {
            _logger.LogInformation("=== EDIT POST METHOD CALLED ===");
            _logger.LogInformation("Product ID: {ProductId}, Name: {ProductName}", id, product?.Name ?? "null");
            _logger.LogInformation("ImageFile: {ImageFileName}, Size: {Size}",
                ImageFile?.FileName ?? "null", ImageFile?.Length ?? 0);

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("ModelState is valid, processing product update");

                    // Store the current image URL before getting existing product
                    string? currentImageUrl = product.ImageUrl;

                    // Get existing product data for image handling (but don't track it)
                    var existingProduct = await _productService.GetProductByIdAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Store existing image URL for potential cleanup
                    string? existingImageUrl = existingProduct.ImageUrl;

                    // Handle image upload
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        _logger.LogInformation("Processing new image upload");

                        if (_fileUploadService.IsValidImageFile(ImageFile))
                        {
                            _logger.LogInformation("Image file is valid, uploading...");

                            // Upload new image first
                            product.ImageUrl = await _fileUploadService.UploadImageAsync(ImageFile, "products");
                            _logger.LogInformation("New image uploaded successfully: {ImageUrl}", product.ImageUrl);

                            // Delete old image after successful upload (if it exists and is different)
                            if (!string.IsNullOrEmpty(existingImageUrl) && existingImageUrl != product.ImageUrl)
                            {
                                await _fileUploadService.DeleteImageAsync(existingImageUrl);
                                _logger.LogInformation("Old image deleted: {OldImageUrl}", existingImageUrl);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Invalid image file: {FileName}", ImageFile.FileName);
                            ModelState.AddModelError("ImageFile", "Please select a valid image file (JPG, PNG, GIF, WEBP) under 5MB.");
                            await PopulateCategoriesDropDown(product.CategoryId);
                            return View(product);
                        }
                    }
                    else
                    {
                        // Keep existing image if no new one is uploaded
                        product.ImageUrl = existingImageUrl;
                        _logger.LogInformation("No new image uploaded, keeping existing: {ImageUrl}", product.ImageUrl);
                    }

                    // Set UpdatedAt timestamp
                    product.UpdatedAt = DateTime.UtcNow;

                    _logger.LogInformation("Updating product in database...");
                    _logger.LogInformation("Product before update: Name={Name}, Price={Price}, CategoryId={CategoryId}, ImageUrl={ImageUrl}",
                        product.Name, product.Price, product.CategoryId, product.ImageUrl);

                    await _productService.UpdateProductAsync(product);
                    _logger.LogInformation("Product updated successfully: {ProductId}", product.Id);

                    TempData["SuccessMessage"] = "Product updated successfully!";
                    _logger.LogInformation("Redirecting to Index with success message");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating product: {ProductName}", product?.Name);
                    ModelState.AddModelError("", "An error occurred while updating the product: " + ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("ModelState is invalid for Edit. Errors: {Errors}",
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            }

            _logger.LogInformation("Returning to Edit view due to validation errors or exception");
            await PopulateCategoriesDropDown(product.CategoryId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Get the product to delete its image
                var product = await _productService.GetProductByIdAsync(id);
                if (product != null && !string.IsNullOrEmpty(product.ImageUrl))
                {
                    await _fileUploadService.DeleteImageAsync(product.ImageUrl);
                }

                await _productService.DeleteProductAsync(id);
                TempData["SuccessMessage"] = "Product deleted successfully.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Cannot delete product. It may be part of existing orders.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Test endpoint for file upload debugging
        [HttpPost]
        public async Task<IActionResult> TestFileUpload(IFormFile? testFile)
        {
            _logger.LogInformation("TestFileUpload called with file: {FileName}", testFile?.FileName ?? "null");

            if (testFile != null)
            {
                _logger.LogInformation("File details: Name={Name}, Size={Size}, ContentType={ContentType}",
                    testFile.FileName, testFile.Length, testFile.ContentType);

                try
                {
                    var result = await _fileUploadService.UploadImageAsync(testFile, "test");
                    _logger.LogInformation("Upload successful: {Result}", result);
                    return Json(new { success = true, url = result });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Upload failed");
                    return Json(new { success = false, error = ex.Message });
                }
            }

            return Json(new { success = false, error = "No file provided" });
        }

        // Simple test endpoint to check if controller is accessible
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Test()
        {
            _logger.LogInformation("Test endpoint called - Controller is accessible");
            return Json(new { success = true, message = "ProductsController is working", timestamp = DateTime.Now });
        }

        // Test POST endpoint to check form submission
        [HttpPost]
        [AllowAnonymous]
        public IActionResult TestPost(string testData)
        {
            _logger.LogInformation("TestPost called with data: {TestData}", testData ?? "null");
            return Json(new { success = true, receivedData = testData, timestamp = DateTime.Now });
        }

        // Debug page to test all functionality
        [HttpGet]
        public IActionResult Debug()
        {
            _logger.LogInformation("Debug page accessed by user: {User}", User.Identity?.Name ?? "Anonymous");
            return View();
        }

        private async Task PopulateCategoriesDropDown(object selectedCategory = null)
        {
            var categories = await _categoryService.GetActiveCategoriesAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", selectedCategory);
        }
    }
}
