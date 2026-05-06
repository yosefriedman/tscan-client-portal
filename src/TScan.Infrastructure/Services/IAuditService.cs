namespace TScan.Infrastructure.Services;

public interface IAuditService
{
    Task LogActionAsync(Guid userId, Guid firmId, string action, string entityType, string? entityId, bool success, string? details = null);
    Task<List<object>> GetAuditLogsAsync(Guid firmId, DateTime? from = null, DateTime? to = null);
}
