using CafeApp.Core.Entities;
using CafeApp.Core.Enums;
using CafeApp.Core.Interfaces;

namespace CafeApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _unitOfWork.Orders.GetAllAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _unitOfWork.Orders.FindAsync(o => o.Status == status);
        }

        public async Task<IEnumerable<Order>> GetOrdersByTableAsync(int tableNumber)
        {
            return await _unitOfWork.Orders.FindAsync(o => o.TableNumber == tableNumber);
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _unitOfWork.Orders.GetByIdAsync(id);
        }

        public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
        {
            return await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (string.IsNullOrEmpty(order.OrderNumber))
            {
                order.OrderNumber = await GenerateOrderNumberAsync();
            }

            order.CreatedAt = DateTime.UtcNow;
            order.TotalAmount = await CalculateOrderTotalAsync(order.OrderItems);

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            order.UpdatedAt = DateTime.UtcNow;
            order.TotalAmount = await CalculateOrderTotalAsync(order.OrderItems);

            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await GetOrderByIdAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found", nameof(orderId));

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            if (status == OrderStatus.Completed)
            {
                order.CompletedAt = DateTime.UtcNow;
            }

            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return order;
        }

        public async Task DeleteOrderAsync(int id)
        {
            await _unitOfWork.Orders.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> OrderExistsAsync(int id)
        {
            return await _unitOfWork.Orders.ExistsAsync(id);
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            var orderCount = await _unitOfWork.Orders.CountAsync(o => o.CreatedAt.Date == DateTime.UtcNow.Date);
            return $"ORD-{today}-{(orderCount + 1):D4}";
        }

        public Task<decimal> CalculateOrderTotalAsync(IEnumerable<OrderItem> orderItems)
        {
            decimal total = 0;
            foreach (var item in orderItems)
            {
                item.TotalPrice = item.UnitPrice * item.Quantity;
                total += item.TotalPrice;
            }
            return Task.FromResult(total);
        }
    }
}
