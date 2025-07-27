using LegalVibes.Application.Contracts.Project;
using LegalVibes.Application.Interfaces;
using LegalVibes.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LegalVibes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectController> _logger;

    public ProjectController(IProjectService projectService, ILogger<ProjectController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects([FromQuery] string? search = null)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("Invalid user context");
        }

        var result = string.IsNullOrWhiteSpace(search) 
            ? await _projectService.GetUserProjectsAsync(userId)
            : await _projectService.SearchAsync(search, userId);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("Invalid user context");
        }

        var result = await _projectService.GetByIdAsync(id, userId);
        
        if (!result.Success)
        {
            var message = result.Error ?? "Unknown error";
            if (message.Contains("not found") || message.Contains("Access denied"))
            {
                return NotFound(new { message });
            }
            return BadRequest(new { message });
        }

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("Invalid user context");
        }

        _logger.LogInformation("Creating project for user {UserId}", userId);

        var result = await _projectService.CreateAsync(request, userId);
        
        if (!result.Success)
        {
            var message = result.Error ?? "Unknown error";
            _logger.LogWarning("Project creation failed for user {UserId}: {Message}", userId, message);
            return BadRequest(new { message });
        }

        _logger.LogInformation("Project created successfully with ID {ProjectId} for user {UserId}", result.Data?.Id, userId);
        return CreatedAtAction(nameof(GetProject), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("Invalid user context");
        }

        _logger.LogInformation("Updating project {ProjectId} for user {UserId}", id, userId);

        var result = await _projectService.UpdateAsync(id, request, userId);
        
        if (!result.Success)
        {
            var message = result.Error ?? "Unknown error";
            _logger.LogWarning("Project update failed for project {ProjectId} by user {UserId}: {Message}", id, userId, message);
            
            if (message.Contains("not found") || message.Contains("Access denied"))
            {
                return NotFound(new { message });
            }
            return BadRequest(new { message });
        }

        _logger.LogInformation("Project {ProjectId} updated successfully by user {UserId}", id, userId);
        return Ok(result.Data);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateProjectStatus(Guid id, [FromBody] UpdateProjectStatusRequest request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("Invalid user context");
        }

        _logger.LogInformation("Updating project {ProjectId} status to {Status} for user {UserId}", id, request.Status, userId);

        var result = await _projectService.UpdateStatusAsync(id, request.Status, userId);
        
        if (!result.Success)
        {
            var message = result.Error ?? "Unknown error";
            _logger.LogWarning("Project status update failed for project {ProjectId} by user {UserId}: {Message}", id, userId, message);
            
            if (message.Contains("not found") || message.Contains("Access denied"))
            {
                return NotFound(new { message });
            }
            return BadRequest(new { message });
        }

        _logger.LogInformation("Project {ProjectId} status updated successfully by user {UserId}", id, userId);
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("Invalid user context");
        }

        _logger.LogInformation("Deleting project {ProjectId} for user {UserId}", id, userId);

        var result = await _projectService.DeleteAsync(id, userId);
        
        if (!result.Success)
        {
            var message = result.Error ?? "Unknown error";
            _logger.LogWarning("Project deletion failed for project {ProjectId} by user {UserId}: {Message}", id, userId, message);
            
            if (message.Contains("not found") || message.Contains("Access denied"))
            {
                return NotFound(new { message });
            }
            return BadRequest(new { message });
        }

        _logger.LogInformation("Project {ProjectId} deleted successfully by user {UserId}", id, userId);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

// DTO for status update requests
public class UpdateProjectStatusRequest
{
    public ProjectStatus Status { get; set; }
} 