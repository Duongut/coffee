using CafeApp.Core.Entities;
using CafeApp.Core.Enums;
using CafeApp.Core.Interfaces;
using CafeApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly INotificationService _notificationService;

        public OrdersController(IOrderService orderService, INotificationService notificationService)
        {
            _orderService = orderService;
            _notificationService = notificationService;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index(OrderStatus? status)
        {
            IEnumerable<Order> orders;

            if (status.HasValue)
            {
                orders = await _orderService.GetOrdersByStatusAsync(status.Value);
                ViewBag.CurrentStatus = status.Value;
            }
            else
            {
                orders = await _orderService.GetAllOrdersAsync();
            }

            ViewBag.StatusFilter = Enum.GetValues<OrderStatus>().Select(s => new { Value = (int)s, Text = s.ToString() });
            return View(orders);
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Admin/Orders/UpdateStatus (Form-based)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order != null)
                {
                    await _orderService.UpdateOrderStatusAsync(orderId, status);

                    // Send real-time notification
                    await _notificationService.SendOrderStatusUpdateAsync(order.OrderNumber, status);

                    // Send table notification
                    var statusMessage = GetStatusMessage(status);
                    await _notificationService.SendTableNotificationAsync(
                        order.TableNumber,
                        $"Order {order.OrderNumber}: {statusMessage}",
                        "info");
                }

                TempData["SuccessMessage"] = "Order status updated successfully.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the order status.";
            }

            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        // POST: Admin/Orders/UpdateStatusAjax (JSON-based for AJAX)
        [HttpPost]
        [Route("Admin/Orders/UpdateStatusAjax")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateStatusAjax([FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                if (!Enum.TryParse<OrderStatus>(request.Status, out var status))
                {
                    return Json(new { success = false, message = "Invalid status value" });
                }

                var order = await _orderService.GetOrderByIdAsync(request.OrderId);
                if (order != null)
                {
                    await _orderService.UpdateOrderStatusAsync(request.OrderId, status);

                    // Send real-time notification
                    await _notificationService.SendOrderStatusUpdateAsync(order.OrderNumber, status);

                    // Send table notification
                    var statusMessage = GetStatusMessage(status);
                    await _notificationService.SendTableNotificationAsync(
                        order.TableNumber,
                        $"Order {order.OrderNumber}: {statusMessage}",
                        "info");

                    // Send kitchen update
                    await _notificationService.SendKitchenUpdateAsync(
                        $"Order {order.OrderNumber} status updated to {status}",
                        new { orderId = order.Id, status = status.ToString() });
                }

                return Json(new { success = true, message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the order status" });
            }
        }

        // GET: Admin/Orders/Kitchen
        public async Task<IActionResult> Kitchen()
        {
            var pendingOrders = await _orderService.GetOrdersByStatusAsync(OrderStatus.Pending);
            var preparingOrders = await _orderService.GetOrdersByStatusAsync(OrderStatus.Preparing);
            var readyOrders = await _orderService.GetOrdersByStatusAsync(OrderStatus.Ready);

            var kitchenOrders = pendingOrders
                .Concat(preparingOrders)
                .Concat(readyOrders)
                .OrderBy(o => o.CreatedAt);

            return View(kitchenOrders);
        }

        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);
                TempData["SuccessMessage"] = "Order deleted successfully.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the order.";
            }
            return RedirectToAction(nameof(Index));
        }

        private string GetStatusMessage(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Order received and is being reviewed",
                OrderStatus.Confirmed => "Order confirmed and being prepared",
                OrderStatus.Preparing => "Your order is being prepared",
                OrderStatus.Ready => "Your order is ready for pickup!",
                OrderStatus.Completed => "Order completed",
                _ => "Status updated"
            };
        }
    }

    public class UpdateOrderStatusRequest
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
