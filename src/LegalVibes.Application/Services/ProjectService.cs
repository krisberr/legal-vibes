using LegalVibes.Application.Common;
using LegalVibes.Application.Contracts.Client;
using LegalVibes.Application.Contracts.Project;
using LegalVibes.Application.Interfaces;
using LegalVibes.Domain.Entities;
using LegalVibes.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace LegalVibes.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(IUnitOfWork unitOfWork, ILogger<ProjectService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BaseResponse<ProjectDto>> GetByIdAsync(Guid id, Guid userId)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            
            if (project == null)
            {
                return BaseResponse<ProjectDto>.ErrorResult("Project not found");
            }

            // Ensure user owns this project
            if (project.UserId != userId)
            {
                _logger.LogWarning("Unauthorized access attempt to project {ProjectId} by user {UserId}", id, userId);
                return BaseResponse<ProjectDto>.ErrorResult("Access denied");
            }

            // Get the client information
            var client = await _unitOfWork.Clients.GetByIdAsync(project.ClientId);
            
            var projectDto = MapToDto(project, client);
            return BaseResponse<ProjectDto>.SuccessResult(projectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project {ProjectId} for user {UserId}", id, userId);
            return BaseResponse<ProjectDto>.ErrorResult("An error occurred while retrieving the project");
        }
    }

    public async Task<BaseResponse<IEnumerable<ProjectDto>>> GetUserProjectsAsync(Guid userId)
    {
        try
        {
            var projects = await _unitOfWork.Projects.FindAsync(p => p.UserId == userId);
            var projectDtos = new List<ProjectDto>();

            foreach (var project in projects)
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(project.ClientId);
                projectDtos.Add(MapToDto(project, client));
            }

            return BaseResponse<IEnumerable<ProjectDto>>.SuccessResult(projectDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects for user {UserId}", userId);
            return BaseResponse<IEnumerable<ProjectDto>>.ErrorResult("An error occurred while retrieving projects");
        }
    }

    public async Task<BaseResponse<ProjectDto>> CreateAsync(CreateProjectRequest request, Guid userId)
    {
        try
        {
            // Validate request
            var validationResult = ValidateCreateRequest(request);
            if (!validationResult.Success)
            {
                return BaseResponse<ProjectDto>.ErrorResult(validationResult.Error ?? "Validation failed");
            }

            // Verify client exists and belongs to user
            var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId);
            if (client == null)
            {
                return BaseResponse<ProjectDto>.ErrorResult("Client not found");
            }

            if (client.UserId != userId)
            {
                _logger.LogWarning("Attempt to create project for unauthorized client {ClientId} by user {UserId}", request.ClientId, userId);
                return BaseResponse<ProjectDto>.ErrorResult("Access denied - client not found");
            }

            // Create project
            var project = new Project
            {
                Name = request.Name,
                Description = request.Description,
                Type = request.Type,
                Status = ProjectStatus.Draft, // Default status
                DueDate = request.DueDate,
                ReferenceNumber = request.ReferenceNumber,
                UserId = userId,
                ClientId = request.ClientId,
                TrademarkName = request.TrademarkName,
                TrademarkDescription = request.TrademarkDescription,
                GoodsAndServices = request.GoodsAndServices,
                SpecialConsiderations = request.SpecialConsiderations,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Project created successfully with ID {ProjectId} for user {UserId}", project.Id, userId);

            var projectDto = MapToDto(project, client);
            return BaseResponse<ProjectDto>.SuccessResult(projectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project for user {UserId}", userId);
            return BaseResponse<ProjectDto>.ErrorResult("An error occurred while creating the project");
        }
    }

    public async Task<BaseResponse<ProjectDto>> UpdateAsync(Guid id, UpdateProjectRequest request, Guid userId)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            
            if (project == null)
            {
                return BaseResponse<ProjectDto>.ErrorResult("Project not found");
            }

            if (project.UserId != userId)
            {
                _logger.LogWarning("Unauthorized update attempt on project {ProjectId} by user {UserId}", id, userId);
                return BaseResponse<ProjectDto>.ErrorResult("Access denied");
            }

            // Validate request
            var validationResult = ValidateUpdateRequest(request);
            if (!validationResult.Success)
            {
                return BaseResponse<ProjectDto>.ErrorResult(validationResult.Error ?? "Validation failed");
            }

            // If ClientId is being changed, verify the new client
            if (request.ClientId.HasValue && request.ClientId.Value != project.ClientId)
            {
                var newClient = await _unitOfWork.Clients.GetByIdAsync(request.ClientId.Value);
                if (newClient == null || newClient.UserId != userId)
                {
                    return BaseResponse<ProjectDto>.ErrorResult("Invalid client selected");
                }
                project.ClientId = request.ClientId.Value;
            }

            // Update fields if provided
            if (!string.IsNullOrWhiteSpace(request.Name))
                project.Name = request.Name;
            
            if (!string.IsNullOrWhiteSpace(request.Description))
                project.Description = request.Description;
            
            if (request.Status.HasValue)
                project.Status = request.Status.Value;
            
            if (request.DueDate.HasValue)
                project.DueDate = request.DueDate.Value;
            
            if (!string.IsNullOrWhiteSpace(request.ReferenceNumber))
                project.ReferenceNumber = request.ReferenceNumber;

            // Update trademark-specific fields
            if (!string.IsNullOrWhiteSpace(request.TrademarkName))
                project.TrademarkName = request.TrademarkName;
            
            if (!string.IsNullOrWhiteSpace(request.TrademarkDescription))
                project.TrademarkDescription = request.TrademarkDescription;
            
            if (!string.IsNullOrWhiteSpace(request.GoodsAndServices))
                project.GoodsAndServices = request.GoodsAndServices;
            
            if (!string.IsNullOrWhiteSpace(request.SpecialConsiderations))
                project.SpecialConsiderations = request.SpecialConsiderations;

            project.LastModifiedAt = DateTime.UtcNow;

            await _unitOfWork.Projects.UpdateAsync(project);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Project {ProjectId} updated successfully by user {UserId}", id, userId);

            // Get updated client info
            var client = await _unitOfWork.Clients.GetByIdAsync(project.ClientId);
            var projectDto = MapToDto(project, client);
            
            return BaseResponse<ProjectDto>.SuccessResult(projectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project {ProjectId} for user {UserId}", id, userId);
            return BaseResponse<ProjectDto>.ErrorResult("An error occurred while updating the project");
        }
    }

    public async Task<BaseResponse<bool>> DeleteAsync(Guid id, Guid userId)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            
            if (project == null)
            {
                return BaseResponse<bool>.ErrorResult("Project not found");
            }

            if (project.UserId != userId)
            {
                _logger.LogWarning("Unauthorized delete attempt on project {ProjectId} by user {UserId}", id, userId);
                return BaseResponse<bool>.ErrorResult("Access denied");
            }

            // Check if project has documents
            var documents = await _unitOfWork.Documents.FindAsync(d => d.ProjectId == id);
            if (documents.Any())
            {
                return BaseResponse<bool>.ErrorResult("Cannot delete project with existing documents. Please delete documents first.");
            }

            await _unitOfWork.Projects.DeleteAsync(project);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Project {ProjectId} deleted successfully by user {UserId}", id, userId);
            return BaseResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project {ProjectId} for user {UserId}", id, userId);
            return BaseResponse<bool>.ErrorResult("An error occurred while deleting the project");
        }
    }

    public async Task<BaseResponse<ProjectDto>> UpdateStatusAsync(Guid id, ProjectStatus status, Guid userId)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            
            if (project == null)
            {
                return BaseResponse<ProjectDto>.ErrorResult("Project not found");
            }

            if (project.UserId != userId)
            {
                _logger.LogWarning("Unauthorized status update attempt on project {ProjectId} by user {UserId}", id, userId);
                return BaseResponse<ProjectDto>.ErrorResult("Access denied");
            }

            project.Status = status;
            project.LastModifiedAt = DateTime.UtcNow;

            await _unitOfWork.Projects.UpdateAsync(project);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Project {ProjectId} status updated to {Status} by user {UserId}", id, status, userId);

            // Get client info
            var client = await _unitOfWork.Clients.GetByIdAsync(project.ClientId);
            var projectDto = MapToDto(project, client);
            
            return BaseResponse<ProjectDto>.SuccessResult(projectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for project {ProjectId} by user {UserId}", id, userId);
            return BaseResponse<ProjectDto>.ErrorResult("An error occurred while updating the project status");
        }
    }

    public async Task<BaseResponse<IEnumerable<ProjectDto>>> SearchAsync(string searchTerm, Guid userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetUserProjectsAsync(userId);
            }

            var searchTermLower = searchTerm.ToLower();
            var projects = await _unitOfWork.Projects.FindAsync(p => 
                p.UserId == userId && 
                (p.Name.ToLower().Contains(searchTermLower) ||
                 p.Description.ToLower().Contains(searchTermLower) ||
                 (p.ReferenceNumber != null && p.ReferenceNumber.ToLower().Contains(searchTermLower)) ||
                 (p.TrademarkName != null && p.TrademarkName.ToLower().Contains(searchTermLower))));

            var projectDtos = new List<ProjectDto>();
            foreach (var project in projects)
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(project.ClientId);
                projectDtos.Add(MapToDto(project, client));
            }

            return BaseResponse<IEnumerable<ProjectDto>>.SuccessResult(projectDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching projects for user {UserId} with term {SearchTerm}", userId, searchTerm);
            return BaseResponse<IEnumerable<ProjectDto>>.ErrorResult("An error occurred while searching projects");
        }
    }

    private static ProjectDto MapToDto(Project project, Client? client)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            Type = project.Type,
            DueDate = project.DueDate,
            ReferenceNumber = project.ReferenceNumber,
            CreatedAt = project.CreatedAt,
            CreatedBy = project.CreatedBy,
            ClientId = project.ClientId,
            Client = client != null ? new ClientDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                CompanyName = client.CompanyName,
                Address = client.Address,
                IsActive = client.IsActive,
                CreatedAt = client.CreatedAt
            } : null!,
            TrademarkName = project.TrademarkName,
            TrademarkDescription = project.TrademarkDescription,
            GoodsAndServices = project.GoodsAndServices,
            SpecialConsiderations = project.SpecialConsiderations
        };
    }

    private static BaseResponse<bool> ValidateCreateRequest(CreateProjectRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BaseResponse<bool>.ErrorResult("Project name is required");

        if (request.Name.Length < 2)
            return BaseResponse<bool>.ErrorResult("Project name must be at least 2 characters long");

        if (request.Name.Length > 200)
            return BaseResponse<bool>.ErrorResult("Project name cannot exceed 200 characters");

        if (request.ClientId == Guid.Empty)
            return BaseResponse<bool>.ErrorResult("Client is required");

        return BaseResponse<bool>.SuccessResult(true);
    }

    private static BaseResponse<bool> ValidateUpdateRequest(UpdateProjectRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            if (request.Name.Length < 2)
                return BaseResponse<bool>.ErrorResult("Project name must be at least 2 characters long");

            if (request.Name.Length > 200)
                return BaseResponse<bool>.ErrorResult("Project name cannot exceed 200 characters");
        }

        return BaseResponse<bool>.SuccessResult(true);
    }
} 