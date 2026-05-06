using Microsoft.EntityFrameworkCore;
using TScan.Infrastructure.Data;
using TScan.Web.Models;

namespace TScan.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        // Generate order number
        order.OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        order.SubmittedAt = DateTime.UtcNow;
        order.Status = "Submitted";

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> GetOrderAsync(Guid orderId, Guid firmId)
    {
        return await _context.Orders
            .Include(o => o.Matter)
            .Include(o => o.Documents)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.FirmId == firmId);
    }

    public async Task<List<Order>> GetUserOrdersAsync(Guid userId, Guid firmId)
    {
        return await _context.Orders
            .Include(o => o.Matter)
            .Where(o => o.CreatedByUserId == userId && o.FirmId == firmId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Order>> GetFirmOrdersAsync(Guid firmId)
    {
        return await _context.Orders
            .Include(o => o.Matter)
            .Where(o => o.FirmId == firmId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateOrderAsync(Order order)
    {
        order.UpdatedAt = DateTime.UtcNow;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}
