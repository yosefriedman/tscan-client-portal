using TScan.Web.Models;

namespace TScan.Infrastructure.Services;

public interface IMatterService
{
    Task<Matter?> GetMatterAsync(Guid matterId, Guid firmId);
    Task<List<Matter>> GetFirmMattersAsync(Guid firmId);
    Task<List<Matter>> SearchMattersAsync(Guid firmId, string searchTerm);
    Task<Matter> CreateMatterAsync(Matter matter);
    Task UpdateMatterAsync(Matter matter);
    Task DeleteMatterAsync(Guid matterId, Guid firmId);
}
