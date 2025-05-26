using CafeApp.Core.Entities;
using CafeApp.Core.Enums;
using CafeApp.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CafeApp.Web.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<OrderStatusHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IHubContext<OrderStatusHub> hubContext, ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendOrderStatusUpdateAsync(string orderNumber, OrderStatus status, string? message = null)
        {
            try
            {
                await _hubContext.SendOrderStatusUpdate(orderNumber, status, message);
                _logger.LogInformation("Sent order status update for order {OrderNumber}: {Status}", orderNumber, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order status update for order {OrderNumber}", orderNumber);
            }
        }

        public async Task SendNewOrderNotificationAsync(Order order)
        {
            try
            {
                await _hubContext.SendNewOrderNotification(order);
                _logger.LogInformation("Sent new order notification for order {OrderNumber}", order.OrderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send new order notification for order {OrderNumber}", order.OrderNumber);
            }
        }

        public async Task SendTableNotificationAsync(int tableNumber, string message, string type = "info")
        {
            try
            {
                await _hubContext.SendTableNotification(tableNumber, message, type);
                _logger.LogInformation("Sent table notification to table {TableNumber}: {Message}", tableNumber, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send table notification to table {TableNumber}", tableNumber);
            }
        }

        public async Task SendKitchenUpdateAsync(string message, object? data = null)
        {
            try
            {
                await _hubContext.SendKitchenUpdate(message, data);
                _logger.LogInformation("Sent kitchen update: {Message}", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send kitchen update");
            }
        }
    }
}
