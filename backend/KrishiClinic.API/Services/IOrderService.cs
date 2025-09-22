using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;

namespace KrishiClinic.API.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<Order> CreateOrderAsync(CreateOrderDto orderDto);
        Task<Order> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto statusDto);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<decimal> GetOrderTotalAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status, DateTime? shippedDate, DateTime? deliveredDate);
        Task<int> GetOrderCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetPendingOrderCountAsync();
    }
}
