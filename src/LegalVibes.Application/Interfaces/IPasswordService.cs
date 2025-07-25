namespace LegalVibes.Application.Interfaces;

public interface IPasswordService
{
    /// <summary>
    /// Hashes a plain text password securely
    /// </summary>
    /// <param name="password">The plain text password to hash</param>
    /// <returns>Hashed password string</returns>
    string HashPassword(string password);
    
    /// <summary>
    /// Verifies a password against its hash
    /// </summary>
    /// <param name="password">The plain text password to verify</param>
    /// <param name="passwordHash">The stored password hash</param>
    /// <returns>True if password matches, false otherwise</returns>
    bool VerifyPassword(string password, string passwordHash);
    
    /// <summary>
    /// Validates password strength requirements
    /// </summary>
    /// <param name="password">The password to validate</param>
    /// <returns>Tuple with validation result and error message if invalid</returns>
    (bool IsValid, string ErrorMessage) ValidatePasswordStrength(string password);
} 