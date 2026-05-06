using TScan.Web.Models;

namespace TScan.Infrastructure.Services;

public interface IDocumentService
{
    Task<Document> UploadDocumentAsync(Guid orderId, IFormFile file, Guid uploadedByUserId);
    Task<Document?> GetDocumentAsync(Guid documentId, Guid firmId);
    Task<List<Document>> GetOrderDocumentsAsync(Guid orderId);
    Task DeleteDocumentAsync(Guid documentId, Guid firmId);
    Task<byte[]> DownloadDocumentAsync(Guid documentId, Guid firmId);
}
