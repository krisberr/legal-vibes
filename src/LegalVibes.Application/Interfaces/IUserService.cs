using LegalVibes.Application.Common;
using LegalVibes.Application.Contracts.User;

namespace LegalVibes.Application.Interfaces;

public interface IUserService
{
    Task<BaseResponse<UserDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<UserDto>> GetByEmailAsync(string email);
    Task<BaseResponse<IEnumerable<UserDto>>> GetAllAsync();
    Task<BaseResponse<UserDto>> CreateAsync(CreateUserRequest request);
    Task<BaseResponse<UserDto>> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<BaseResponse<bool>> DeleteAsync(Guid id);
    Task<BaseResponse<AuthResponse>> AuthenticateAsync(AuthRequest request);
} 