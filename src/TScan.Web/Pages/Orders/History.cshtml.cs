using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TScan.Infrastructure.Services;
using TScan.Web.Models;

namespace TScan.Web.Pages.Orders;

[Authorize]
public class OrderHistoryModel : PageModel
{
    private readonly IOrderService _orderService;

    public OrderHistoryModel(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public List<Order> Orders { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var firmId = Guid.Parse(User.FindFirst("FirmId")?.Value ?? Guid.Empty.ToString());

        Orders = await _orderService.GetUserOrdersAsync(userId, firmId);
    }
}
