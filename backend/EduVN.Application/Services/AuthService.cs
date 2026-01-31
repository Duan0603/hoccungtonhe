using EduVN.Application.DTOs.Auth;
using EduVN.Application.Interfaces;
using EduVN.Application.Settings;
using EduVN.Domain.Entities;
using EduVN.Domain.Enums;
using Microsoft.Extensions.Options;

namespace EduVN.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly IGoogleAuthService _googleAuthService;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        IOptions<JwtSettings> jwtSettings,
        IGoogleAuthService googleAuthService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings.Value;
        _googleAuthService = googleAuthService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new ArgumentException("Email already registered");
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

        await _userRepository.AddAsync(user);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || user.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check if user is approved
        if (user.Status != UserStatus.Approved)
        {
            throw new UnauthorizedAccessException($"Account is {user.Status}. Please contact admin.");
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        // Find refresh token
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = storedToken.User;

        // Revoke old refresh token
        storedToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(storedToken);

        // Generate new tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // Save new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

        return new AuthResponse(
            accessToken,
            newRefreshToken,
            new UserDto(user.Id, user.Email, user.FullName, user.Role.ToString(),
                       user.Status.ToString(), user.Grade, user.School)
        );
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (storedToken != null)
        {
            storedToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(storedToken);
        }
    }

    public async Task<AuthResponse> GoogleLoginAsync(string idToken)
    {
        // Verify ID token with Google
        var googleUser = await _googleAuthService.VerifyIdTokenAsync(idToken);
        
        if (googleUser == null)
        {
            throw new UnauthorizedAccessException("Invalid Google token");
        }

        // Find existing user by GoogleId or Email
        var user = await _userRepository.GetByGoogleIdAsync(googleUser.GoogleId);
        if (user == null)
        {
             user = await _userRepository.GetByEmailAsync(googleUser.Email);
        }

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

            await _userRepository.AddAsync(user);
        }
        else if (user.GoogleId == null)
        {
            // Link existing email account with Google
            user.GoogleId = googleUser.GoogleId;
            await _userRepository.UpdateAsync(user);
        }

        // Check if user is approved
        if (user.Status != UserStatus.Approved)
        {
            throw new UnauthorizedAccessException($"Account is {user.Status}. Please contact admin.");
        }

        return await GenerateAuthResponseAsync(user);
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
    {
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);

        return new AuthResponse(
            accessToken,
            refreshToken,
            new UserDto(user.Id, user.Email, user.FullName, user.Role.ToString(),
                       user.Status.ToString(), user.Grade, user.School)
        );
    }
}
