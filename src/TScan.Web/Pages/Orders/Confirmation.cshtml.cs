using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TScan.Infrastructure.Services;
using TScan.Web.Models;

namespace TScan.Web.Pages.Orders;

[Authorize]
public class ConfirmationModel : PageModel
{
    private readonly IOrderService _orderService;

    public ConfirmationModel(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public Order? Order { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid orderId)
    {
        var firmId = Guid.Parse(User.FindFirst("FirmId")?.Value ?? Guid.Empty.ToString());
        Order = await _orderService.GetOrderAsync(orderId, firmId);

        if (Order == null)
            return NotFound();

        return Page();
    }
}
