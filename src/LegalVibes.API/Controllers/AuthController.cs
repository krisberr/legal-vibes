using LegalVibes.Application.Contracts.User;
using LegalVibes.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LegalVibes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="request">User registration details</param>
    /// <returns>Created user information</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", request.Email);
        
        var result = await _userService.CreateAsync(request);
        
        if (!result.Success)
        {
            _logger.LogWarning("Registration failed for email {Email}: {Error}", request.Email, result.Error);
            return BadRequest(new { error = result.Error });
        }

        _logger.LogInformation("User registered successfully: {Email}", request.Email);
        return CreatedAtAction(nameof(GetProfile), new { }, new 
        { 
            success = true, 
            message = result.Message, 
            data = result.Data 
        });
    }

    /// <summary>
    /// Authenticate user and return JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);
        
        var result = await _userService.AuthenticateAsync(request);
        
        if (!result.Success)
        {
            _logger.LogWarning("Login failed for email {Email}: {Error}", request.Email, result.Error);
            return Unauthorized(new { error = result.Error });
        }

        _logger.LogInformation("User logged in successfully: {Email}", request.Email);
        return Ok(new 
        { 
            success = true, 
            message = result.Message, 
            data = result.Data 
        });
    }

    /// <summary>
    /// Get current user profile (requires authentication)
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Invalid user ID in token claims");
            return Unauthorized(new { error = "Invalid token" });
        }

        var result = await _userService.GetByIdAsync(userId);
        
        if (!result.Success)
        {
            _logger.LogWarning("Profile retrieval failed for user {UserId}: {Error}", userId, result.Error);
            return NotFound(new { error = result.Error });
        }

        return Ok(new 
        { 
            success = true, 
            data = result.Data 
        });
    }

    /// <summary>
    /// Update current user profile (requires authentication)
    /// </summary>
    /// <param name="request">Updated user information</param>
    /// <returns>Updated user information</returns>
    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Invalid user ID in token claims");
            return Unauthorized(new { error = "Invalid token" });
        }

        _logger.LogInformation("Profile update attempt for user: {UserId}", userId);
        
        var result = await _userService.UpdateAsync(userId, request);
        
        if (!result.Success)
        {
            _logger.LogWarning("Profile update failed for user {UserId}: {Error}", userId, result.Error);
            return BadRequest(new { error = result.Error });
        }

        _logger.LogInformation("Profile updated successfully for user: {UserId}", userId);
        return Ok(new 
        { 
            success = true, 
            message = result.Message, 
            data = result.Data 
        });
    }

    /// <summary>
    /// Validate current JWT token
    /// </summary>
    /// <returns>Token validation result</returns>
    [HttpPost("validate-token")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public IActionResult ValidateToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(emailClaim))
        {
            return Unauthorized(new { error = "Invalid token" });
        }

        return Ok(new 
        { 
            success = true, 
            message = "Token is valid",
            data = new 
            {
                userId = userIdClaim,
                email = emailClaim
            }
        });
    }

    /// <summary>
    /// Refresh JWT token (placeholder for future implementation)
    /// </summary>
    /// <returns>New JWT token</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            // Get token from Authorization header
            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { error = "No token provided" });
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            // Use IJwtService to validate expired token (structure check only)
            var jwtService = HttpContext.RequestServices.GetRequiredService<IJwtService>();
            var principal = jwtService.ValidateExpiredToken(token);
            
            if (principal == null)
            {
                return Unauthorized(new { error = "Invalid token structure" });
            }

            // Extract user ID from token
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { error = "Invalid user ID in token" });
            }

            // Get fresh user data from database
            var userResult = await _userService.GetByIdAsync(userId);
            if (!userResult.Success || userResult.Data == null)
            {
                return Unauthorized(new { error = "User not found or inactive" });
            }

            var user = userResult.Data;
            
            // Check if user is still active
            if (!user.IsActive)
            {
                return Unauthorized(new { error = "User account is inactive" });
            }

            // Generate new token
            var newToken = jwtService.GenerateToken(new Domain.Entities.User
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
            });

            _logger.LogInformation("Token refreshed successfully for user: {Email}", user.Email);

            return Ok(new
            {
                success = true,
                message = "Token refreshed successfully",
                data = new
                {
                    token = newToken,
                    user = user
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                error = "An error occurred while refreshing the token" 
            });
        }
    }
} 