using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CafeApp.Web.Models;
using CafeApp.Core.Interfaces;

namespace CafeApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public HomeController(ILogger<HomeController> logger, IProductService productService, ICategoryService categoryService)
    {
        _logger = logger;
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var featuredProducts = await _productService.GetFeaturedProductsAsync();
        var categories = await _categoryService.GetActiveCategoriesAsync();

        ViewBag.FeaturedProducts = featuredProducts;
        ViewBag.Categories = categories;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
