using LegalVibes.Domain.Entities;

namespace LegalVibes.Application.Interfaces;

public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for the given user
    /// </summary>
    /// <param name="user">The user to generate token for</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(User user);
    
    /// <summary>
    /// Validates a JWT token and extracts user claims
    /// </summary>
    /// <param name="token">The JWT token to validate</param>
    /// <returns>ClaimsPrincipal if valid, null if invalid</returns>
    System.Security.Claims.ClaimsPrincipal? ValidateToken(string token);
    
    /// <summary>
    /// Gets the user ID from a JWT token
    /// </summary>
    /// <param name="token">The JWT token</param>
    /// <returns>User ID if valid, null if invalid</returns>
    Guid? GetUserIdFromToken(string token);
    
    /// <summary>
    /// Checks if a JWT token is expired
    /// </summary>
    /// <param name="token">The JWT token to check</param>
    /// <returns>True if expired, false if still valid</returns>
    bool IsTokenExpired(string token);
} 