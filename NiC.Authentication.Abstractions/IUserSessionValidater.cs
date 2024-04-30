using NiC.Authentication.Models.Dtos;

namespace NiC.Authentication.Abstractions;

public interface IUserSessionValidater
{
    Task<string> ValidateRefreshTokenAsync(RefreshTokenModel refreshTokenModel);

    Task<string> ValidateTokenAsync(LoginWithTokenModel loginWithTokenModel);
}