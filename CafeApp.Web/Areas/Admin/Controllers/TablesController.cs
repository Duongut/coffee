using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CafeApp.Core.Interfaces;

namespace CafeApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class TablesController : Controller
    {
        private readonly IOrderService _orderService;
        private const int MAX_TABLES = 20;

        public TablesController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: Admin/Tables
        public async Task<IActionResult> Index()
        {
            var tableStatuses = new List<TableStatusViewModel>();

            for (int i = 1; i <= MAX_TABLES; i++)
            {
                var hasActiveOrders = await HasActiveOrdersAsync(i);
                tableStatuses.Add(new TableStatusViewModel
                {
                    TableNumber = i,
                    IsOccupied = hasActiveOrders,
                    Status = hasActiveOrders ? "Occupied" : "Available"
                });
            }

            return View(tableStatuses);
        }

        private async Task<bool> HasActiveOrdersAsync(int tableNumber)
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return orders.Any(o => o.TableNumber == tableNumber && 
                             (o.Status == Core.Enums.OrderStatus.Pending || 
                              o.Status == Core.Enums.OrderStatus.Confirmed || 
                              o.Status == Core.Enums.OrderStatus.Preparing || 
                              o.Status == Core.Enums.OrderStatus.Ready));
        }
    }

    public class TableStatusViewModel
    {
        public int TableNumber { get; set; }
        public bool IsOccupied { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? LastActivity { get; set; }
        public string? CurrentOrder { get; set; }
    }
}
