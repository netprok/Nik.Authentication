namespace Nik.Authentication;

internal sealed class UserSessionValidator(
    ITokenUnpacker tokenUnpacker,
    IUserLoader userLoader,
    ILogger<UserSessionValidator> logger) : IUserSessionValidater
{
    private const string Bearer = "bearer ";

    public async Task<string> ValidateTokenAsync(LoginWithTokenModel model)
    {
        if (model.Token.ToLower().StartsWith(Bearer))
        {
            model.Token = model.Token[Bearer.Length..];
        }

        var username = tokenUnpacker.GetUsername(model.Token);
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"Tried to login with invalid token.");
            return string.Empty;
        }

        var user = await userLoader.LoadByNameAsync(username);

        if (user == default)
        {
            logger.LogWarning($"Tried to login with token with invalid username: {username}");
            return string.Empty;
        }

        var querySession = user.Sessions.Where(session =>
            session.MachineName.ToLower() == model.MachineName.ToLower() &&
            session.Deadline > DateTime.UtcNow &&
            session.AccessToken == model.Token);
        if (!querySession.Any())
        {
            logger.LogInformation($"Login with token failed for user: {username}.");
            return string.Empty;
        }

        return username;
    }

    public async Task<string> ValidateRefreshTokenAsync(RefreshTokenModel refreshTokenModel)
    {
        var username = tokenUnpacker.GetUsername(refreshTokenModel.Token);
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"Tried to refresh token with invalid token.");
            return string.Empty;
        }

        var user = await userLoader.LoadByNameAsync(username);

        if (user == default)
        {
            logger.LogWarning($"Tried to refresh token with invalid username: {username}");
            return string.Empty;
        }

        var querySession = user.Sessions.Where(session =>
                               session.MachineName.ToLower() == refreshTokenModel.MachineName.ToLower() &&
                               session.RefreshDeadline > DateTime.UtcNow &&
                               session.RefreshToken == refreshTokenModel.Token);
        if (!querySession.Any())
        {
            logger.LogInformation($"Refresh token not valid for user: {username} on machine: {refreshTokenModel.MachineName}.");
            return string.Empty;
        }

        return username;
    }
}