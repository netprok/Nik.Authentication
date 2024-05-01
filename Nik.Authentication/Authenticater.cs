namespace Nik.Authentication;

internal sealed class Authenticater(
    IUserSessionValidater userSessionValidater,
    IUserSessionWriter userSessionWriter,
    IAuthenticationValidater authenticationValidater,
    IObjectMapper objectMapper,
    ILogger<Authenticater> logger) : IAuthenticater
{
    public async Task<ViewUserSessionModel?> LoginAsync(LoginModel loginModel)
    {
        if (!await authenticationValidater.IsLoginValidAsync(loginModel))
        {
            return null;
        }

        logger.LogInformation($"Successful login of the user: {loginModel.Username} on machine: {loginModel.MachineName}.");
        return await userSessionWriter.GenerateAsync(loginModel.Username, loginModel.MachineName);
    }

    public async Task<string> LoginWithTokenAsync(LoginWithTokenModel loginWithTokenModel)
    {
        var username = await userSessionValidater.ValidateTokenAsync(loginWithTokenModel);
        if (string.IsNullOrWhiteSpace(username))
        {
            return string.Empty;
        }

        logger.LogInformation($"Successful login with token on machine: {loginWithTokenModel.MachineName}.");
        return username;
    }

    public async Task<ViewUserSessionModel?> RefreshTokenAsync(RefreshTokenModel refreshTokenModel)
    {
        var username = await userSessionValidater.ValidateRefreshTokenAsync(refreshTokenModel);
        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }

        logger.LogInformation($"Refreshing token for the user: {username} on machine: {refreshTokenModel.MachineName}...");
        return await userSessionWriter.GenerateAsync(username, refreshTokenModel.MachineName);
    }

    public async Task<bool> LogoutAsync(LogoutModel logoutModel)
    {
        var loginWithTokenModel = objectMapper.Map<LoginWithTokenModel>(logoutModel);
        var username = await userSessionValidater.ValidateTokenAsync(loginWithTokenModel);
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogInformation($"Login with token failed on machine: {logoutModel.MachineName}.");
            return false;
        }

        try
        {
            await userSessionWriter.DeleteAsync(username, logoutModel.Token, logoutModel.MachineName);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Logout failed for the user: {username} on machine: {logoutModel.MachineName}.");
            return false;
        }

        logger.LogInformation($"Successfully logged out of the user: {username} on machine: {logoutModel.MachineName}.");
        return true;
    }
}