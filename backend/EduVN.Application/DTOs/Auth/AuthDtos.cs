using System.ComponentModel.DataAnnotations;

namespace EduVN.Application.DTOs.Auth;

public record RegisterRequest(
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password,
    [Required] string FullName,
    int? Grade, // For students
    string? School // For students
);

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

public record RefreshTokenRequest(
    [Required] string RefreshToken
);

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    UserDto User
);

public record UserDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    string Status,
    int? Grade,
    string? School
);
