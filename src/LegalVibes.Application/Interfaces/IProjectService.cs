using LegalVibes.Application.Common;
using LegalVibes.Application.Contracts.Project;
using LegalVibes.Domain.Enums;

namespace LegalVibes.Application.Interfaces;

public interface IProjectService
{
    Task<BaseResponse<ProjectDto>> GetByIdAsync(Guid id, Guid userId);
    Task<BaseResponse<IEnumerable<ProjectDto>>> GetUserProjectsAsync(Guid userId);
    Task<BaseResponse<ProjectDto>> CreateAsync(CreateProjectRequest request, Guid userId);
    Task<BaseResponse<ProjectDto>> UpdateAsync(Guid id, UpdateProjectRequest request, Guid userId);
    Task<BaseResponse<bool>> DeleteAsync(Guid id, Guid userId);
    Task<BaseResponse<ProjectDto>> UpdateStatusAsync(Guid id, ProjectStatus status, Guid userId);
    Task<BaseResponse<IEnumerable<ProjectDto>>> SearchAsync(string searchTerm, Guid userId);
} 