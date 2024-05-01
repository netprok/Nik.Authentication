using Nik.Authentication.Models.Dtos;

namespace Nik.Authentication.Abstractions;

public interface IUserSessionValidater
{
    Task<string> ValidateRefreshTokenAsync(RefreshTokenModel refreshTokenModel);

    Task<string> ValidateTokenAsync(LoginWithTokenModel loginWithTokenModel);
}