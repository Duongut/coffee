using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using CafeApp.Core.Entities;
using CafeApp.Core.Enums;

namespace CafeApp.Web.Hubs
{
    public class OrderStatusHub : Hub
    {
        private readonly ILogger<OrderStatusHub> _logger;

        public OrderStatusHub(ILogger<OrderStatusHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinOrderGroup(string orderNumber)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Order_{orderNumber}");
            _logger.LogInformation("Client {ConnectionId} joined order group {OrderNumber}", Context.ConnectionId, orderNumber);
        }

        public async Task LeaveOrderGroup(string orderNumber)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Order_{orderNumber}");
            _logger.LogInformation("Client {ConnectionId} left order group {OrderNumber}", Context.ConnectionId, orderNumber);
        }

        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task JoinKitchenGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Kitchen");
            _logger.LogInformation("Staff member {ConnectionId} joined kitchen group", Context.ConnectionId);
        }

        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task LeaveKitchenGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Kitchen");
            _logger.LogInformation("Staff member {ConnectionId} left kitchen group", Context.ConnectionId);
        }

        public async Task JoinTableGroup(int tableNumber)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Table_{tableNumber}");
            _logger.LogInformation("Client {ConnectionId} joined table group {TableNumber}", Context.ConnectionId, tableNumber);
        }

        public async Task LeaveTableGroup(int tableNumber)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Table_{tableNumber}");
            _logger.LogInformation("Client {ConnectionId} left table group {TableNumber}", Context.ConnectionId, tableNumber);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client {ConnectionId} connected to OrderStatusHub", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client {ConnectionId} disconnected from OrderStatusHub", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }

    // Extension methods for sending notifications
    public static class OrderStatusHubExtensions
    {
        public static async Task SendOrderStatusUpdate(this IHubContext<OrderStatusHub> hubContext,
            string orderNumber, OrderStatus status, string? message = null)
        {
            await hubContext.Clients.Group($"Order_{orderNumber}").SendAsync("OrderStatusUpdated", new
            {
                orderNumber,
                status = status.ToString(),
                message,
                timestamp = DateTime.UtcNow
            });
        }

        public static async Task SendNewOrderNotification(this IHubContext<OrderStatusHub> hubContext,
            Order order)
        {
            await hubContext.Clients.Group("Kitchen").SendAsync("NewOrderReceived", new
            {
                orderId = order.Id,
                orderNumber = order.OrderNumber,
                tableNumber = order.TableNumber,
                customerName = order.CustomerName,
                totalAmount = order.TotalAmount,
                itemCount = order.OrderItems.Count,
                createdAt = order.CreatedAt,
                estimatedTime = GetEstimatedTime(order)
            });
        }

        public static async Task SendTableNotification(this IHubContext<OrderStatusHub> hubContext,
            int tableNumber, string message, string type = "info")
        {
            await hubContext.Clients.Group($"Table_{tableNumber}").SendAsync("TableNotification", new
            {
                tableNumber,
                message,
                type,
                timestamp = DateTime.UtcNow
            });
        }

        public static async Task SendKitchenUpdate(this IHubContext<OrderStatusHub> hubContext,
            string message, object? data = null)
        {
            await hubContext.Clients.Group("Kitchen").SendAsync("KitchenUpdate", new
            {
                message,
                data,
                timestamp = DateTime.UtcNow
            });
        }

        private static int GetEstimatedTime(Order order)
        {
            // Simple estimation: 5 minutes base + 2 minutes per item
            return 5 + (order.OrderItems.Count * 2);
        }
    }
}
