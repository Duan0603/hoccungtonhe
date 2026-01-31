using EduVN.Application.Interfaces;
using Google.Apis.Auth;

namespace EduVN.Infrastructure.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly string _clientId;

    public GoogleAuthService(string clientId)
    {
        _clientId = clientId;
    }

    public async Task<GoogleUserInfo?> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _clientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new GoogleUserInfo(
                payload.Subject,    // Google User ID
                payload.Email,
                payload.Name ?? payload.Email.Split('@')[0],
                payload.Picture
            );
        }
        catch (InvalidJwtException)
        {
            // Token is invalid
            return null;
        }
        catch (Exception)
        {
            // Other errors (network, etc.)
            return null;
        }
    }
}
