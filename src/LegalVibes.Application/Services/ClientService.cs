using LegalVibes.Application.Common;
using LegalVibes.Application.Contracts.Client;
using LegalVibes.Application.Interfaces;
using LegalVibes.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace LegalVibes.Application.Services;

public class ClientService : IClientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClientService> _logger;

    public ClientService(IUnitOfWork unitOfWork, ILogger<ClientService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BaseResponse<ClientDto>> GetByIdAsync(Guid id, Guid userId)
    {
        try
        {
            var clients = await _unitOfWork.Clients.FindAsync(c => c.Id == id && c.UserId == userId);
            var client = clients.FirstOrDefault();

            if (client == null)
            {
                _logger.LogWarning("Client with ID {ClientId} not found for user {UserId}", id, userId);
                return BaseResponse<ClientDto>.ErrorResult("Client not found");
            }

            var clientDto = MapToDto(client);
            return BaseResponse<ClientDto>.SuccessResult(clientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving client with ID {ClientId} for user {UserId}", id, userId);
            return BaseResponse<ClientDto>.ErrorResult("An error occurred while retrieving the client");
        }
    }

    public async Task<BaseResponse<IEnumerable<ClientDto>>> GetAllAsync(Guid userId)
    {
        try
        {
            var clients = await _unitOfWork.Clients.FindAsync(c => c.UserId == userId && c.IsActive);
            var clientDtos = clients.Select(MapToDto);

            return BaseResponse<IEnumerable<ClientDto>>.SuccessResult(clientDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving clients for user {UserId}", userId);
            return BaseResponse<IEnumerable<ClientDto>>.ErrorResult("An error occurred while retrieving clients");
        }
    }

    public async Task<BaseResponse<ClientDto>> CreateAsync(CreateClientRequest request, Guid userId)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Name))
                return BaseResponse<ClientDto>.ErrorResult("Client name is required");

            if (string.IsNullOrWhiteSpace(request.Email))
                return BaseResponse<ClientDto>.ErrorResult("Client email is required");

            // Basic email validation
            if (!request.Email.Contains('@'))
                return BaseResponse<ClientDto>.ErrorResult("Please enter a valid email address");

            // Check for duplicate client email for this user
            var existingClients = await _unitOfWork.Clients.FindAsync(c => 
                c.UserId == userId && c.Email.ToLower() == request.Email.ToLower() && c.IsActive);
            
            if (existingClients.Any())
            {
                _logger.LogWarning("Attempt to create duplicate client with email {Email} for user {UserId}", request.Email, userId);
                return BaseResponse<ClientDto>.ErrorResult("A client with this email already exists");
            }

            // Create new client
            var client = new Client
            {
                Name = request.Name.Trim(),
                Email = request.Email.ToLower().Trim(),
                PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty,
                CompanyName = request.CompanyName?.Trim(),
                Address = request.Address?.Trim(),
                UserId = userId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System" // TODO: Get from current user context
            };

            await _unitOfWork.Clients.AddAsync(client);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Client created successfully with ID {ClientId} for user {UserId}", client.Id, userId);

            var clientDto = MapToDto(client);
            return BaseResponse<ClientDto>.SuccessResult(clientDto, "Client created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client for user {UserId}", userId);
            return BaseResponse<ClientDto>.ErrorResult("An error occurred while creating the client");
        }
    }

    public async Task<BaseResponse<ClientDto>> UpdateAsync(Guid id, UpdateClientRequest request, Guid userId)
    {
        try
        {
            var clients = await _unitOfWork.Clients.FindAsync(c => c.Id == id && c.UserId == userId);
            var client = clients.FirstOrDefault();

            if (client == null)
            {
                _logger.LogWarning("Attempt to update non-existent client with ID {ClientId} for user {UserId}", id, userId);
                return BaseResponse<ClientDto>.ErrorResult("Client not found");
            }

            // Update provided fields
            if (!string.IsNullOrWhiteSpace(request.Name))
                client.Name = request.Name.Trim();

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                if (!request.Email.Contains('@'))
                    return BaseResponse<ClientDto>.ErrorResult("Please enter a valid email address");
                client.Email = request.Email.ToLower().Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                client.PhoneNumber = request.PhoneNumber.Trim();

            if (request.CompanyName != null)
                client.CompanyName = string.IsNullOrWhiteSpace(request.CompanyName) ? null : request.CompanyName.Trim();

            if (request.Address != null)
                client.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();

            if (request.IsActive.HasValue)
                client.IsActive = request.IsActive.Value;

            client.LastModifiedAt = DateTime.UtcNow;
            client.LastModifiedBy = "System"; // TODO: Get from current user context

            await _unitOfWork.Clients.UpdateAsync(client);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Client updated successfully with ID {ClientId} for user {UserId}", id, userId);

            var clientDto = MapToDto(client);
            return BaseResponse<ClientDto>.SuccessResult(clientDto, "Client updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating client with ID {ClientId} for user {UserId}", id, userId);
            return BaseResponse<ClientDto>.ErrorResult("An error occurred while updating the client");
        }
    }

    public async Task<BaseResponse<bool>> DeleteAsync(Guid id, Guid userId)
    {
        try
        {
            var clients = await _unitOfWork.Clients.FindAsync(c => c.Id == id && c.UserId == userId);
            var client = clients.FirstOrDefault();

            if (client == null)
            {
                _logger.LogWarning("Attempt to delete non-existent client with ID {ClientId} for user {UserId}", id, userId);
                return BaseResponse<bool>.ErrorResult("Client not found");
            }

            // Check if client has active projects
            var activeProjects = await _unitOfWork.Projects.FindAsync(p => p.ClientId == id && p.Status != Domain.Enums.ProjectStatus.Archived);
            if (activeProjects.Any())
            {
                _logger.LogWarning("Attempt to delete client with ID {ClientId} that has active projects", id);
                return BaseResponse<bool>.ErrorResult("Cannot delete client with active projects. Please archive or reassign projects first.");
            }

            // Soft delete by marking as inactive
            client.IsActive = false;
            client.LastModifiedAt = DateTime.UtcNow;
            client.LastModifiedBy = "System"; // TODO: Get from current user context

            await _unitOfWork.Clients.UpdateAsync(client);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Client soft-deleted successfully with ID {ClientId} for user {UserId}", id, userId);

            return BaseResponse<bool>.SuccessResult(true, "Client deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting client with ID {ClientId} for user {UserId}", id, userId);
            return BaseResponse<bool>.ErrorResult("An error occurred while deleting the client");
        }
    }

    private static ClientDto MapToDto(Client client)
    {
        return new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber,
            CompanyName = client.CompanyName,
            Address = client.Address,
            IsActive = client.IsActive,
            CreatedAt = client.CreatedAt
        };
    }
} 