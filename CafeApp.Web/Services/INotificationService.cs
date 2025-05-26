using CafeApp.Core.Entities;
using CafeApp.Core.Enums;

namespace CafeApp.Web.Services
{
    public interface INotificationService
    {
        Task SendOrderStatusUpdateAsync(string orderNumber, OrderStatus status, string? message = null);
        Task SendNewOrderNotificationAsync(Order order);
        Task SendTableNotificationAsync(int tableNumber, string message, string type = "info");
        Task SendKitchenUpdateAsync(string message, object? data = null);
    }
}
