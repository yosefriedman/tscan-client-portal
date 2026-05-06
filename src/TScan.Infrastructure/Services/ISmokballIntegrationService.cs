using TScan.Web.Models;

namespace TScan.Infrastructure.Services;

public interface ISmokballIntegrationService
{
    Task<List<Matter>> SyncMattersFromSmokballAsync(Guid firmId, string tenantId);
    Task<Matter?> GetMatterFromSmokballAsync(string matterId);
}
