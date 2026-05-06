using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TScan.Infrastructure.Data;
using TScan.Web.Models;

namespace TScan.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogActionAsync(Guid userId, Guid firmId, string action, string entityType, string? entityId, bool success, string? details = null)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

        var auditLog = new AuditLog
        {
            UserId = userId,
            FirmId = firmId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            IpAddress = ipAddress,
            Success = success
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }

    public async Task<List<object>> GetAuditLogsAsync(Guid firmId, DateTime? from = null, DateTime? to = null)
    {
        var query = _context.AuditLogs
            .Where(log => log.FirmId == firmId);

        if (from.HasValue)
            query = query.Where(log => log.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(log => log.CreatedAt <= to.Value);

        return await query
            .OrderByDescending(log => log.CreatedAt)
            .Select(log => new
            {
                log.Id,
                log.Action,
                log.EntityType,
                log.EntityId,
                log.Success,
                log.CreatedAt
            })
            .Cast<object>()
            .ToListAsync();
    }
}
