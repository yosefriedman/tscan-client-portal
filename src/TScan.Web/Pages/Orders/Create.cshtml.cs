using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TScan.Infrastructure.Services;
using TScan.Web.Models;

namespace TScan.Web.Pages.Orders;

[Authorize]
public class CreateOrderModel : PageModel
{
    private readonly IMatterService _matterService;
    private readonly IOrderService _orderService;
    private readonly IDocumentService _documentService;
    private readonly IAuditService _auditService;

    public CreateOrderModel(
        IMatterService matterService,
        IOrderService orderService,
        IDocumentService documentService,
        IAuditService auditService)
    {
        _matterService = matterService;
        _orderService = orderService;
        _documentService = documentService;
        _auditService = auditService;
    }

    public Matter? Matter { get; set; }
    [BindProperty]
    public Order Order { get; set; } = new();
    [BindProperty]
    public List<IFormFile>? Documents { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid matterId)
    {
        var firmId = Guid.Parse(User.FindFirst("FirmId")?.Value ?? Guid.Empty.ToString());
        Matter = await _matterService.GetMatterAsync(matterId, firmId);

        if (Matter == null)
            return NotFound();

        // Log matter access
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        await _auditService.LogActionAsync(userId, firmId, "MatterAccess", "Matter", matterId.ToString(), true);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid matterId, string? dateStart, string? dateEnd)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var firmId = Guid.Parse(User.FindFirst("FirmId")?.Value ?? Guid.Empty.ToString());

        // Validate matter access
        Matter = await _matterService.GetMatterAsync(matterId, firmId);
        if (Matter == null)
            return NotFound();

        Order.MatterId = matterId;
        Order.FirmId = firmId;
        Order.CreatedByUserId = userId;

        // Parse dates
        if (DateTime.TryParse(dateStart, out var start))
            Order.DateRangeStart = start;
        if (DateTime.TryParse(dateEnd, out var end))
            Order.DateRangeEnd = end;

        // Create order
        var createdOrder = await _orderService.CreateOrderAsync(Order);

        // Handle document uploads
        if (Documents?.Any() ?? false)
        {
            foreach (var file in Documents)
            {
                if (file.Length > 0)
                {
                    await _documentService.UploadDocumentAsync(createdOrder.Id, file, userId);
                }
            }
        }

        // Log order creation
        await _auditService.LogActionAsync(userId, firmId, "OrderCreated", "Order", createdOrder.Id.ToString(), true);

        return RedirectToPage("/Orders/Confirmation", new { orderId = createdOrder.Id });
    }
}
