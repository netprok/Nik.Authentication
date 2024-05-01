using Nik.Authentication.Models.Dtos;

namespace Nik.Authentication.Abstractions;

public interface IAuthenticater
{
    Task<ViewUserSessionModel?> LoginAsync(LoginModel loginModel);

    Task<string> LoginWithTokenAsync(LoginWithTokenModel loginWithTokenModel);

    Task<ViewUserSessionModel?> RefreshTokenAsync(RefreshTokenModel refreshTokenModel);

    Task<bool> LogoutAsync(LogoutModel logoutModel);
}