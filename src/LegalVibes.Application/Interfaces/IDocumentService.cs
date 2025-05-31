using LegalVibes.Application.Common;
using LegalVibes.Application.Contracts.Document;
using LegalVibes.Domain.Enums;

namespace LegalVibes.Application.Interfaces;

public interface IDocumentService
{
    Task<BaseResponse<DocumentDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<IEnumerable<DocumentDto>>> GetAllAsync();
    Task<BaseResponse<IEnumerable<DocumentDto>>> GetProjectDocumentsAsync(Guid projectId);
    Task<BaseResponse<DocumentDto>> CreateAsync(CreateDocumentRequest request);
    Task<BaseResponse<DocumentDto>> UpdateAsync(Guid id, UpdateDocumentRequest request);
    Task<BaseResponse<bool>> DeleteAsync(Guid id);
    Task<BaseResponse<DocumentDto>> UpdateStatusAsync(Guid id, DocumentStatus status);
    Task<BaseResponse<byte[]>> DownloadAsync(Guid id);
    Task<BaseResponse<DocumentDto>> GenerateAIDocumentAsync(GenerateAIDocumentRequest request);
} 