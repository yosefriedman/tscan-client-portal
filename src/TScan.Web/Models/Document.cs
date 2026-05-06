namespace TScan.Web.Models;

public class Document
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty; // Authorization, Subpoena, Notice, etc.
    public string ContentHash { get; set; } = string.Empty; // For integrity verification
    public bool IsEncrypted { get; set; } = true;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public Guid UploadedByUserId { get; set; }

    // Navigation
    public virtual Order? Order { get; set; }
    public virtual ApplicationUser? UploadedByUser { get; set; }
}
