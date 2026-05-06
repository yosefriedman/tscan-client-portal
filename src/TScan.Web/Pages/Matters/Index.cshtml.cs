using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TScan.Infrastructure.Services;
using TScan.Web.Models;

namespace TScan.Web.Pages.Matters;

[Authorize]
public class MatterListModel : PageModel
{
    private readonly IMatterService _matterService;

    public MatterListModel(IMatterService matterService)
    {
        _matterService = matterService;
    }

    public List<Matter> Matters { get; set; } = new();
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync(string? searchTerm)
    {
        SearchTerm = searchTerm;
        var firmId = Guid.Parse(User.FindFirst("FirmId")?.Value ?? Guid.Empty.ToString());
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            Matters = await _matterService.SearchMattersAsync(firmId, searchTerm);
        }
        else
        {
            Matters = await _matterService.GetFirmMattersAsync(firmId);
        }
    }
}
