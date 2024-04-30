using NiC.Authentication.Models.Dtos;

namespace NiC.Authentication.Abstractions;

public interface IAuthenticater
{
    Task<ViewUserSessionModel?> LoginAsync(LoginModel loginModel);

    Task<string> LoginWithTokenAsync(LoginWithTokenModel loginWithTokenModel);

    Task<ViewUserSessionModel?> RefreshTokenAsync(RefreshTokenModel refreshTokenModel);

    Task<bool> LogoutAsync(LogoutModel logoutModel);
}