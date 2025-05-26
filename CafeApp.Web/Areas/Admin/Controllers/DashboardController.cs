using CafeApp.Core.Enums;
using CafeApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class DashboardController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public DashboardController(IOrderService orderService, IProductService productService, ICategoryService categoryService)
        {
            _orderService = orderService;
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = new DashboardViewModel
            {
                TotalOrders = (await _orderService.GetAllOrdersAsync()).Count(),
                TotalProducts = (await _productService.GetAllProductsAsync()).Count(),
                TotalCategories = (await _categoryService.GetAllCategoriesAsync()).Count(),
                PendingOrders = (await _orderService.GetOrdersByStatusAsync(OrderStatus.Pending)).Count(),
                PreparingOrders = (await _orderService.GetOrdersByStatusAsync(OrderStatus.Preparing)).Count(),
                ReadyOrders = (await _orderService.GetOrdersByStatusAsync(OrderStatus.Ready)).Count(),
                CompletedOrders = (await _orderService.GetOrdersByStatusAsync(OrderStatus.Completed)).Count(),
                TodayRevenue = await CalculateTodayRevenue(),
                RecentOrders = (await _orderService.GetAllOrdersAsync())
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(5)
                    .ToList()
            };

            return View(dashboardData);
        }

        private async Task<decimal> CalculateTodayRevenue()
        {
            var todayOrders = (await _orderService.GetAllOrdersAsync())
                .Where(o => o.CreatedAt.Date == DateTime.Today && o.Status == OrderStatus.Completed);
            
            return todayOrders.Sum(o => o.TotalAmount);
        }
    }

    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int PendingOrders { get; set; }
        public int PreparingOrders { get; set; }
        public int ReadyOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TodayRevenue { get; set; }
        public List<CafeApp.Core.Entities.Order> RecentOrders { get; set; } = new();
    }
}
