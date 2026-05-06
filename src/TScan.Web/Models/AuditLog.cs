namespace TScan.Web.Models;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? FirmId { get; set; }
    public string Action { get; set; } = string.Empty; // Login, MatterAccess, OrderCreated, DocumentUploaded
    public string EntityType { get; set; } = string.Empty; // User, Matter, Order, Document
    public string? EntityId { get; set; }
    public string? Details { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Success { get; set; } = true;

    // Navigation
    public virtual ApplicationUser? User { get; set; }
    public virtual Firm? Firm { get; set; }
}
