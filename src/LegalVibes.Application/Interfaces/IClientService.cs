using LegalVibes.Application.Common;
using LegalVibes.Application.Contracts.Client;

namespace LegalVibes.Application.Interfaces;

public interface IClientService
{
    Task<BaseResponse<ClientDto>> GetByIdAsync(Guid id, Guid userId);
    Task<BaseResponse<IEnumerable<ClientDto>>> GetAllAsync(Guid userId);
    Task<BaseResponse<ClientDto>> CreateAsync(CreateClientRequest request, Guid userId);
    Task<BaseResponse<ClientDto>> UpdateAsync(Guid id, UpdateClientRequest request, Guid userId);
    Task<BaseResponse<bool>> DeleteAsync(Guid id, Guid userId);
} 