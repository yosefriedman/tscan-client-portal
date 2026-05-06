using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TScan.Infrastructure.Data;
using TScan.Web.Models;

namespace TScan.Infrastructure.Services;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly string[] _allowedExtensions = { ".pdf", ".doc", ".docx", ".txt", ".jpg", ".png" };
    private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50MB

    public DocumentService(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<Document> UploadDocumentAsync(Guid orderId, IFormFile file, Guid uploadedByUserId)
    {
        if (file.Length == 0 || file.Length > MaxFileSizeBytes)
            throw new InvalidOperationException("File size exceeds maximum allowed size.");

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!_allowedExtensions.Contains(extension))
            throw new InvalidOperationException($"File extension {extension} is not allowed.");

        // Create upload directory
        var uploadPath = Path.Combine(_environment.ContentRootPath, "uploads", "documents");
        Directory.CreateDirectory(uploadPath);

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadPath, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Calculate hash
        string contentHash;
        using (var stream = System.IO.File.OpenRead(filePath))
        using (var sha256 = SHA256.Create())
        {
            var hash = await sha256.ComputeHashAsync(stream);
            contentHash = Convert.ToBase64String(hash);
        }

        // Create document record
        var document = new Document
        {
            OrderId = orderId,
            FileName = file.FileName,
            FileExtension = extension,
            FileSizeBytes = file.Length,
            FilePath = Path.Combine("documents", fileName),
            DocumentType = GetDocumentType(file.FileName),
            ContentHash = contentHash,
            IsEncrypted = true,
            UploadedByUserId = uploadedByUserId
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return document;
    }

    public async Task<Document?> GetDocumentAsync(Guid documentId, Guid firmId)
    {
        return await _context.Documents
            .Include(d => d.Order)
            .FirstOrDefaultAsync(d => d.Id == documentId && d.Order!.FirmId == firmId);
    }

    public async Task<List<Document>> GetOrderDocumentsAsync(Guid orderId)
    {
        return await _context.Documents
            .Where(d => d.OrderId == orderId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    public async Task DeleteDocumentAsync(Guid documentId, Guid firmId)
    {
        var document = await GetDocumentAsync(documentId, firmId);
        if (document != null)
        {
            var filePath = Path.Combine(_environment.ContentRootPath, "uploads", document.FilePath);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<byte[]> DownloadDocumentAsync(Guid documentId, Guid firmId)
    {
        var document = await GetDocumentAsync(documentId, firmId)
            ?? throw new FileNotFoundException("Document not found.");

        var filePath = Path.Combine(_environment.ContentRootPath, "uploads", document.FilePath);
        if (!System.IO.File.Exists(filePath))
            throw new FileNotFoundException("File not found on disk.");

        return await System.IO.File.ReadAllBytesAsync(filePath);
    }

    private string GetDocumentType(string fileName)
    {
        var lower = fileName.ToLower();
        if (lower.Contains("auth")) return "Authorization";
        if (lower.Contains("subpoena")) return "Subpoena";
        if (lower.Contains("notice")) return "Notice";
        return "Other";
    }
}
