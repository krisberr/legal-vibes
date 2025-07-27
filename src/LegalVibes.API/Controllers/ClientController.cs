using LegalVibes.Application.Contracts.Client;
using LegalVibes.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LegalVibes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly ILogger<ClientController> _logger;

    public ClientController(IClientService clientService, ILogger<ClientController> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetClients()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("Invalid user context");
        }

        var result = await _clientService.GetAllAsync(userId);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClient(Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("Invalid user context");
        }

        var result = await _clientService.GetByIdAsync(id, userId);
        
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
    public async Task<IActionResult> CreateClient([FromBody] CreateClientRequest request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("Invalid user context");
        }

        _logger.LogInformation("Creating client for user {UserId}", userId);

        var result = await _clientService.CreateAsync(request, userId);
        
        if (!result.Success)
        {
            var message = result.Error ?? "Unknown error";
            _logger.LogWarning("Client creation failed for user {UserId}: {Message}", userId, message);
            return BadRequest(new { message });
        }

        _logger.LogInformation("Client created successfully with ID {ClientId} for user {UserId}", result.Data?.Id, userId);
        return CreatedAtAction(nameof(GetClient), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(Guid id, [FromBody] UpdateClientRequest request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("Invalid user context");
        }

        _logger.LogInformation("Updating client {ClientId} for user {UserId}", id, userId);

        var result = await _clientService.UpdateAsync(id, request, userId);
        
        if (!result.Success)
        {
            var message = result.Error ?? "Unknown error";
            _logger.LogWarning("Client update failed for client {ClientId} by user {UserId}: {Message}", id, userId, message);
            
            if (message.Contains("not found") || message.Contains("Access denied"))
            {
                return NotFound(new { message });
            }
            return BadRequest(new { message });
        }

        _logger.LogInformation("Client {ClientId} updated successfully by user {UserId}", id, userId);
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("Invalid user context");
        }

        _logger.LogInformation("Deleting client {ClientId} for user {UserId}", id, userId);

        var result = await _clientService.DeleteAsync(id, userId);
        
        if (!result.Success)
        {
            var message = result.Error ?? "Unknown error";
            _logger.LogWarning("Client deletion failed for client {ClientId} by user {UserId}: {Message}", id, userId, message);
            
            if (message.Contains("not found") || message.Contains("Access denied"))
            {
                return NotFound(new { message });
            }
            return BadRequest(new { message });
        }

        _logger.LogInformation("Client {ClientId} deleted successfully by user {UserId}", id, userId);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
} 