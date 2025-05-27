using CafeApp.Core.Entities;
using CafeApp.Core.Enums;
using CafeApp.Core.Interfaces;
using CafeApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CafeApp.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;
        private const string CartSessionKey = "ShoppingCart";

        public OrderController(IOrderService orderService, IProductService productService, INotificationService notificationService)
        {
            _orderService = orderService;
            _productService = productService;
            _notificationService = notificationService;
        }

        // GET: Order/Checkout
        public async Task<IActionResult> Checkout(int? tableNumber)
        {
            // Get table number from session if not provided
            if (!tableNumber.HasValue)
            {
                tableNumber = HttpContext.Session.GetInt32("CurrentTableNumber");
            }

            var cart = GetCartFromSession(tableNumber);
            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Menu");
            }

            var orderItems = new List<OrderItemViewModel>();
            decimal total = 0;

            foreach (var cartItem in cart)
            {
                var product = await _productService.GetProductByIdAsync(cartItem.ProductId);
                if (product != null)
                {
                    var orderItem = new OrderItemViewModel
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        Quantity = cartItem.Quantity,
                        TotalPrice = product.Price * cartItem.Quantity,
                        Notes = cartItem.Notes,
                        Size = cartItem.Size,
                        Temperature = cartItem.Temperature
                    };
                    orderItems.Add(orderItem);
                    total += orderItem.TotalPrice;
                }
            }

            // Validate table number - More flexible approach
            if (!tableNumber.HasValue || tableNumber.Value < 1)
            {
                // If no valid table number, try to get from session
                tableNumber = HttpContext.Session.GetInt32("CurrentTableNumber");

                // If still no valid table number, redirect to cart with guidance
                if (!tableNumber.HasValue || tableNumber.Value < 1)
                {
                    TempData["ErrorMessage"] = "Please select your table first by scanning the QR code at your table to proceed with checkout.";
                    return RedirectToAction("Index", "Cart");
                }

                // Validate table number range
                if (tableNumber.Value > 20)
                {
                    TempData["ErrorMessage"] = "Invalid table number. Please scan the QR code at your table.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            var checkoutViewModel = new CheckoutViewModel
            {
                OrderItems = orderItems,
                SubTotal = total,
                TaxAmount = total * 0.1m, // 10% tax
                TotalAmount = total + (total * 0.1m),
                TableNumber = tableNumber.Value // No more default to 1!
            };

            return View(checkoutViewModel);
        }

        // POST: Order/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Checkout", model);
            }

            var cart = GetCartFromSession(model.TableNumber);
            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Menu");
            }

            try
            {
                var order = new Order
                {
                    TableNumber = model.TableNumber,
                    CustomerName = model.CustomerName,
                    CustomerPhone = model.CustomerPhone,
                    CustomerEmail = model.CustomerEmail,
                    Status = OrderStatus.Pending,
                    Notes = model.Notes,
                    OrderItems = new List<OrderItem>()
                };

                foreach (var cartItem in cart)
                {
                    var product = await _productService.GetProductByIdAsync(cartItem.ProductId);
                    if (product != null)
                    {
                        order.OrderItems.Add(new OrderItem
                        {
                            ProductId = product.Id,
                            Quantity = cartItem.Quantity,
                            UnitPrice = product.Price,
                            TotalPrice = product.Price * cartItem.Quantity,
                            Notes = cartItem.Notes,
                            Size = cartItem.Size,
                            Temperature = cartItem.Temperature
                        });
                    }
                }

                order.SubTotal = order.OrderItems.Sum(oi => oi.TotalPrice);
                order.TaxAmount = order.SubTotal * 0.1m;
                order.TotalAmount = order.SubTotal + order.TaxAmount;

                var createdOrder = await _orderService.CreateOrderAsync(order);

                // Send real-time notification to kitchen
                await _notificationService.SendNewOrderNotificationAsync(createdOrder);

                // Send table notification
                await _notificationService.SendTableNotificationAsync(
                    createdOrder.TableNumber,
                    $"Order {createdOrder.OrderNumber} has been placed successfully!",
                    "success");

                // Clear cart after successful order (use table-specific session key)
                var cartSessionKey = $"{CartSessionKey}_Table_{model.TableNumber}";
                HttpContext.Session.Remove(cartSessionKey);

                TempData["SuccessMessage"] = $"Order placed successfully! Order number: {createdOrder.OrderNumber}";
                return RedirectToAction("Confirmation", new { orderNumber = createdOrder.OrderNumber });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while placing your order. Please try again.";
                return View("Checkout", model);
            }
        }

        // GET: Order/Confirmation
        public async Task<IActionResult> Confirmation(string orderNumber)
        {
            var order = await _orderService.GetOrderByNumberAsync(orderNumber);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Track
        public async Task<IActionResult> Track(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                return View();
            }

            var order = await _orderService.GetOrderByNumberAsync(orderNumber);
            if (order == null)
            {
                ViewBag.ErrorMessage = "Order not found.";
                return View();
            }

            return View(order);
        }

        // GET: Order/RealTimeTrack
        public async Task<IActionResult> RealTimeTrack(string? orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                return View((Order?)null);
            }

            var order = await _orderService.GetOrderByNumberAsync(orderNumber);
            if (order == null)
            {
                ViewBag.ErrorMessage = "Order not found.";
                return View((Order?)null);
            }

            return View(order);
        }

        // GET: Order/GetOrderStatus - AJAX endpoint for status checking
        [HttpGet]
        public async Task<IActionResult> GetOrderStatus(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                return Json(new { success = false, message = "Order number is required" });
            }

            var order = await _orderService.GetOrderByNumberAsync(orderNumber);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

            return Json(new {
                success = true,
                status = order.Status.ToString(),
                estimatedTime = GetEstimatedTime(order.Status),
                createdAt = order.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        private string GetEstimatedTime(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Order received, waiting for confirmation",
                OrderStatus.Confirmed => "15-20 minutes",
                OrderStatus.Preparing => "10-15 minutes",
                OrderStatus.Ready => "Ready for pickup!",
                OrderStatus.Completed => "Order completed",
                _ => "Unknown"
            };
        }

        private List<CartItem> GetCartFromSession(int? tableNumber = null)
        {
            string cartSessionKey;

            if (tableNumber.HasValue && tableNumber.Value > 0)
            {
                // Use table-specific cart session
                cartSessionKey = $"{CartSessionKey}_Table_{tableNumber.Value}";
            }
            else
            {
                // Fallback to general cart (for backward compatibility)
                cartSessionKey = CartSessionKey;
            }

            var cartJson = HttpContext.Session.GetString(cartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonConvert.DeserializeObject<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }
    }

    public class CheckoutViewModel
    {
        public List<OrderItemViewModel> OrderItems { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int TableNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public string? Notes { get; set; }
    }

    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }
        public string? Size { get; set; }
        public string? Temperature { get; set; }
    }
}
