using LegalVibes.Application.Interfaces;
using System.Text.RegularExpressions;

namespace LegalVibes.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    private const int WorkFactor = 12; // BCrypt work factor for good security vs performance balance

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordHash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch
        {
            return false;
        }
    }

    public (bool IsValid, string ErrorMessage) ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrEmpty(password))
            return (false, "Password is required");

        if (password.Length < 8)
            return (false, "Password must be at least 8 characters long");

        if (password.Length > 128)
            return (false, "Password must not exceed 128 characters");

        // Check for at least one uppercase letter
        if (!Regex.IsMatch(password, @"[A-Z]"))
            return (false, "Password must contain at least one uppercase letter");

        // Check for at least one lowercase letter
        if (!Regex.IsMatch(password, @"[a-z]"))
            return (false, "Password must contain at least one lowercase letter");

        // Check for at least one digit
        if (!Regex.IsMatch(password, @"\d"))
            return (false, "Password must contain at least one number");

        // Check for at least one special character
        if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?\"":{}|<>]"))
            return (false, "Password must contain at least one special character (!@#$%^&*(),.?\"\":{}|<>)");

        return (true, string.Empty);
    }
} 