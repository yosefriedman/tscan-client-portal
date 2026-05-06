using TScan.Web.Models;

namespace TScan.Infrastructure.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(Order order);
    Task<Order?> GetOrderAsync(Guid orderId, Guid firmId);
    Task<List<Order>> GetUserOrdersAsync(Guid userId, Guid firmId);
    Task<List<Order>> GetFirmOrdersAsync(Guid firmId);
    Task UpdateOrderAsync(Order order);
}
