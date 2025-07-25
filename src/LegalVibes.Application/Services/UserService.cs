using LegalVibes.Application.Common;
using LegalVibes.Application.Contracts.User;
using LegalVibes.Application.Interfaces;
using LegalVibes.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace LegalVibes.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUnitOfWork unitOfWork, 
        IPasswordService passwordService, 
        IJwtService jwtService,
        ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<BaseResponse<UserDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return BaseResponse<UserDto>.ErrorResult("User not found");
            }

            var userDto = MapToDto(user);
            return BaseResponse<UserDto>.SuccessResult(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            return BaseResponse<UserDto>.ErrorResult("An error occurred while retrieving the user");
        }
    }

    public async Task<BaseResponse<UserDto>> GetByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BaseResponse<UserDto>.ErrorResult("Email is required");
            }

            var users = await _unitOfWork.Users.FindAsync(u => u.Email.ToLower() == email.ToLower());
            var user = users.FirstOrDefault();
            
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return BaseResponse<UserDto>.ErrorResult("User not found");
            }

            var userDto = MapToDto(user);
            return BaseResponse<UserDto>.SuccessResult(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email}", email);
            return BaseResponse<UserDto>.ErrorResult("An error occurred while retrieving the user");
        }
    }

    public async Task<BaseResponse<IEnumerable<UserDto>>> GetAllAsync()
    {
        try
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var userDtos = users.Select(MapToDto);
            
            return BaseResponse<IEnumerable<UserDto>>.SuccessResult(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            return BaseResponse<IEnumerable<UserDto>>.ErrorResult("An error occurred while retrieving users");
        }
    }

    public async Task<BaseResponse<UserDto>> CreateAsync(CreateUserRequest request)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Email))
                return BaseResponse<UserDto>.ErrorResult("Email is required");

            if (string.IsNullOrWhiteSpace(request.Password))
                return BaseResponse<UserDto>.ErrorResult("Password is required");

            // Validate password strength
            var (isValid, errorMessage) = _passwordService.ValidatePasswordStrength(request.Password);
            if (!isValid)
                return BaseResponse<UserDto>.ErrorResult(errorMessage);

            // Check if user already exists
            var existingUsers = await _unitOfWork.Users.FindAsync(u => u.Email.ToLower() == request.Email.ToLower());
            if (existingUsers.Any())
            {
                _logger.LogWarning("Attempt to create user with existing email {Email}", request.Email);
                return BaseResponse<UserDto>.ErrorResult("A user with this email already exists");
            }

            // Create new user
            var user = new User
            {
                Email = request.Email.ToLower().Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty,
                CompanyName = request.CompanyName?.Trim(),
                JobTitle = request.JobTitle?.Trim(),
                PasswordHash = _passwordService.HashPassword(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System" // TODO: Get from current user context when available
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User created successfully with ID {UserId} and email {Email}", user.Id, user.Email);

            var userDto = MapToDto(user);
            return BaseResponse<UserDto>.SuccessResult(userDto, "User created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email {Email}", request.Email);
            return BaseResponse<UserDto>.ErrorResult("An error occurred while creating the user");
        }
    }

    public async Task<BaseResponse<UserDto>> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Attempt to update non-existent user with ID {UserId}", id);
                return BaseResponse<UserDto>.ErrorResult("User not found");
            }

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(request.FirstName))
                user.FirstName = request.FirstName.Trim();
            
            if (!string.IsNullOrWhiteSpace(request.LastName))
                user.LastName = request.LastName.Trim();
            
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber.Trim();
            
            if (request.CompanyName != null)
                user.CompanyName = string.IsNullOrWhiteSpace(request.CompanyName) ? null : request.CompanyName.Trim();
            
            if (request.JobTitle != null)
                user.JobTitle = string.IsNullOrWhiteSpace(request.JobTitle) ? null : request.JobTitle.Trim();

            user.LastModifiedAt = DateTime.UtcNow;
            user.LastModifiedBy = "System"; // TODO: Get from current user context

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User updated successfully with ID {UserId}", id);

            var userDto = MapToDto(user);
            return BaseResponse<UserDto>.SuccessResult(userDto, "User updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", id);
            return BaseResponse<UserDto>.ErrorResult("An error occurred while updating the user");
        }
    }

    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Attempt to delete non-existent user with ID {UserId}", id);
                return BaseResponse<bool>.ErrorResult("User not found");
            }

            // Soft delete by marking as inactive
            user.IsActive = false;
            user.LastModifiedAt = DateTime.UtcNow;
            user.LastModifiedBy = "System"; // TODO: Get from current user context

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User soft-deleted successfully with ID {UserId}", id);

            return BaseResponse<bool>.SuccessResult(true, "User deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            return BaseResponse<bool>.ErrorResult("An error occurred while deleting the user");
        }
    }

    public async Task<BaseResponse<AuthResponse>> AuthenticateAsync(AuthRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Authentication attempt with missing email or password");
                return BaseResponse<AuthResponse>.ErrorResult("Email and password are required");
            }

            // Find user by email
            var users = await _unitOfWork.Users.FindAsync(u => u.Email.ToLower() == request.Email.ToLower());
            var user = users.FirstOrDefault();

            if (user == null)
            {
                _logger.LogWarning("Authentication attempt for non-existent email {Email}", request.Email);
                return BaseResponse<AuthResponse>.ErrorResult("Invalid email or password");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Authentication attempt for inactive user {Email}", request.Email);
                return BaseResponse<AuthResponse>.ErrorResult("User account is inactive");
            }

            // Verify password
            if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed authentication attempt for user {Email}", request.Email);
                return BaseResponse<AuthResponse>.ErrorResult("Invalid email or password");
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(1440); // 24 hours - should match JWT config

            _logger.LogInformation("User {Email} authenticated successfully", request.Email);

            var authResponse = new AuthResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = MapToDto(user)
            };

            return BaseResponse<AuthResponse>.SuccessResult(authResponse, "Authentication successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication for email {Email}", request.Email);
            return BaseResponse<AuthResponse>.ErrorResult("An error occurred during authentication");
        }
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            CompanyName = user.CompanyName,
            JobTitle = user.JobTitle,
            CreatedAt = user.CreatedAt
        };
    }
} 