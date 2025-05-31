using LegalVibes.Application.Common;
using LegalVibes.Application.Contracts.Project;
using LegalVibes.Domain.Enums;

namespace LegalVibes.Application.Interfaces;

public interface IProjectService
{
    Task<BaseResponse<ProjectDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<IEnumerable<ProjectDto>>> GetAllAsync();
    Task<BaseResponse<IEnumerable<ProjectDto>>> GetUserProjectsAsync(Guid userId);
    Task<BaseResponse<ProjectDto>> CreateAsync(CreateProjectRequest request);
    Task<BaseResponse<ProjectDto>> UpdateAsync(Guid id, UpdateProjectRequest request);
    Task<BaseResponse<bool>> DeleteAsync(Guid id);
    Task<BaseResponse<ProjectDto>> UpdateStatusAsync(Guid id, ProjectStatus status);
    Task<BaseResponse<IEnumerable<ProjectDto>>> SearchAsync(string searchTerm);
} 