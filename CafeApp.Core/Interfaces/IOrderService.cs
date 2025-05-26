using CafeApp.Core.Entities;
using CafeApp.Core.Enums;

namespace CafeApp.Core.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByTableAsync(int tableNumber);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order?> GetOrderByNumberAsync(string orderNumber);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> UpdateOrderAsync(Order order);
        Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task DeleteOrderAsync(int id);
        Task<bool> OrderExistsAsync(int id);
        Task<string> GenerateOrderNumberAsync();
        Task<decimal> CalculateOrderTotalAsync(IEnumerable<OrderItem> orderItems);
    }
}
