using CafeApp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CafeApp.Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public MenuController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // GET: Menu
        public async Task<IActionResult> Index(int? categoryId, string searchTerm)
        {
            var categories = await _categoryService.GetActiveCategoriesAsync();
            var products = await _productService.GetAvailableProductsAsync();

            if (categoryId.HasValue)
            {
                products = await _productService.GetProductsByCategoryAsync(categoryId.Value);
                ViewBag.SelectedCategoryId = categoryId.Value;
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                products = await _productService.SearchProductsAsync(searchTerm);
                ViewBag.SearchTerm = searchTerm;
            }

            ViewBag.Categories = categories;
            return View(products);
        }

        // GET: Menu/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null || !product.IsAvailable)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Menu/Table/5 (QR Code access)
        public async Task<IActionResult> Table(int tableNumber)
        {
            if (tableNumber < 1 || tableNumber > 20) // Assuming max 20 tables
            {
                return BadRequest("Invalid table number");
            }

            // Generate unique session for this table
            var tableSessionId = $"TBL_{tableNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}";
            HttpContext.Session.SetString($"TableSession_{tableNumber}", tableSessionId);
            HttpContext.Session.SetInt32("CurrentTableNumber", tableNumber);

            ViewBag.TableNumber = tableNumber;
            ViewBag.TableSessionId = tableSessionId;

            var categories = await _categoryService.GetActiveCategoriesAsync();
            var products = await _productService.GetAvailableProductsAsync();

            ViewBag.Categories = categories;

            return View(products);
        }

        // GET: Menu/Category/5
        public async Task<IActionResult> Category(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null || !category.IsActive)
            {
                return NotFound();
            }

            var products = await _productService.GetProductsByCategoryAsync(id);
            ViewBag.Category = category;

            return View(products);
        }
    }
}
