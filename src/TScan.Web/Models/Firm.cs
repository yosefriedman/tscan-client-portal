namespace TScan.Web.Models;

public class Firm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? BillingReference { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Smokeball Integration
    public string? SmokballApiKey { get; set; }
    public string? SmokballTenantId { get; set; }
    public DateTime? LastMatterSync { get; set; }

    // Navigation
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public virtual ICollection<Matter> Matters { get; set; } = new List<Matter>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
