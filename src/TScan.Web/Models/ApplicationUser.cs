using Microsoft.AspNetCore.Identity;

namespace TScan.Web.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid FirmId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public string? UserRole { get; set; } // "FirmAdmin", "StandardUser", "TScanAdmin"

    // Navigation
    public virtual Firm? Firm { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
