namespace EduVN.Application.Interfaces;

public interface IGoogleAuthService
{
    /// <summary>
    /// Verifies a Google ID token and returns the user info if valid.
    /// </summary>
    /// <param name="idToken">The ID token from Google Sign-In</param>
    /// <returns>GoogleUserInfo if token is valid, null otherwise</returns>
    Task<GoogleUserInfo?> VerifyIdTokenAsync(string idToken);
}

public record GoogleUserInfo(
    string GoogleId,
    string Email,
    string FullName,
    string? ProfilePictureUrl
);
