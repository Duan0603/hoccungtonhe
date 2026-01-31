using EduVN.Application.DTOs.Auth;
using EduVN.Application.Interfaces;
using EduVN.Application.Settings;
using EduVN.Domain.Entities;
using EduVN.Domain.Enums;
using EduVN.Infrastructure.Persistence;
using EduVN.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EduVN.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly IGoogleAuthService _googleAuthService;

    public AuthController(
        ApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        IOptions<JwtSettings> jwtSettings,
        IGoogleAuthService googleAuthService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings.Value;
        _googleAuthService = googleAuthService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "Email already registered" });
        }

        // Create new user
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            Role = UserRole.Student, // Default to student
            Status = UserStatus.Approved, // Auto-approve for now
            Grade = request.Grade,
            School = request.School
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponse(
            accessToken,
            refreshToken,
            new UserDto(user.Id, user.Email, user.FullName, user.Role.ToString(), 
                       user.Status.ToString(), user.Grade, user.School)
        ));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        // Find user by email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || user.PasswordHash == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Check if user is approved
        if (user.Status != UserStatus.Approved)
        {
            return Unauthorized(new { message = $"Account is {user.Status}. Please contact admin." });
        }

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponse(
            accessToken,
            refreshToken,
            new UserDto(user.Id, user.Email, user.FullName, user.Role.ToString(),
                       user.Status.ToString(), user.Grade, user.School)
        ));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenRequest request)
    {
        // Find refresh token
        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        var user = storedToken.User;

        // Generate new tokens
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // Revoke old refresh token
        storedToken.IsRevoked = true;

        // Save new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponse(
            newAccessToken,
            newRefreshToken,
            new UserDto(user.Id, user.Email, user.FullName, user.Role.ToString(),
                       user.Status.ToString(), user.Grade, user.School)
        ));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest request)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (storedToken != null)
        {
            storedToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }

        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("google")]
    public async Task<ActionResult<AuthResponse>> GoogleLogin(GoogleAuthRequest request)
    {
        // Verify ID token with Google
        var googleUser = await _googleAuthService.VerifyIdTokenAsync(request.IdToken);
        
        if (googleUser == null)
        {
            return Unauthorized(new { message = "Invalid Google token" });
        }

        // Find existing user by GoogleId or Email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.GoogleId == googleUser.GoogleId || u.Email == googleUser.Email);

        if (user == null)
        {
            // Create new user
            user = new User
            {
                Email = googleUser.Email,
                FullName = googleUser.FullName,
                GoogleId = googleUser.GoogleId,
                PasswordHash = null, // OAuth users don't have password
                Role = UserRole.Student, // Default to student
                Status = UserStatus.Approved // Auto-approve OAuth users
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        else if (user.GoogleId == null)
        {
            // Link existing email account with Google
            user.GoogleId = googleUser.GoogleId;
            await _context.SaveChangesAsync();
        }

        // Check if user is approved
        if (user.Status != UserStatus.Approved)
        {
            return Unauthorized(new { message = $"Account is {user.Status}. Please contact admin." });
        }

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponse(
            accessToken,
            refreshToken,
            new UserDto(user.Id, user.Email, user.FullName, user.Role.ToString(),
                       user.Status.ToString(), user.Grade, user.School)
        ));
    }
}
