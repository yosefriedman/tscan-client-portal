namespace TScan.Web.Models;

public class Matter
{
    public Guid Id { get; set; }
    public Guid FirmId { get; set; }
    public string SmokballMatterId { get; set; } = string.Empty; // External ID from Smokeball
    public string MatterName { get; set; } = string.Empty;
    public string? ClientName { get; set; }
    public string? FileNumber { get; set; }
    public string? ClaimNumber { get; set; }
    public string? CaseCaption { get; set; }
    public string? ResponsibleAttorney { get; set; }
    public string? AssignedStaff { get; set; }
    public string? MatterStatus { get; set; } // Active, Closed, On Hold, etc.
    public string? FirmOffice { get; set; }
    public string? PlaintiffDefendant { get; set; }
    public string? CourtJurisdiction { get; set; }
    public string? BillingContact { get; set; }
    public string? BillingReference { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSyncedAt { get; set; }

    // Navigation
    public virtual Firm? Firm { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
