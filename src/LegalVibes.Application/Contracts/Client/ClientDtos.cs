namespace LegalVibes.Application.Contracts.Client;

public record ClientDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? CompanyName { get; init; }
    public string? Address { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record CreateClientRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? CompanyName { get; init; }
    public string? Address { get; init; }
}

public record UpdateClientRequest
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? CompanyName { get; init; }
    public string? Address { get; init; }
    public bool? IsActive { get; init; }
} 