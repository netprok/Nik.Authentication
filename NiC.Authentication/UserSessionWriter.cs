namespace NiC.Authentication;

internal sealed class UserSessionWriter(
    IObjectMapper objectMapper,
    ITokenGenerater tokenGenerater,
    IUserLoader userLoader,
    AuthenticationDbContext dbContext) : IUserSessionWriter
{
    public async Task DeleteAsync(string username, string accessToken, string machineName)
    {
        var user = await userLoader.LoadByNameAsync(username);

        user.Sessions.RemoveAll(session => session.AccessToken == accessToken && session.MachineName.ToLower() == machineName.ToLower());
        await dbContext.UpdateAsync(user);
    }

    public async Task<ViewUserSessionModel> GenerateAsync(string username, string machineName)
    {
        DateTime utcNow = DateTime.UtcNow;
        var user = await userLoader.LoadByNameAsync(username);
        user.Sessions.RemoveAll(session => session.MachineName == machineName || session.RefreshDeadline < utcNow);

        var userSessionExpiration = Context.Configuration.GetSection<UserSessionExpiration>();

        UserSession userSession = new()
        {
            CreateTime = utcNow,
            MachineName = machineName,
            Deadline = utcNow.AddMinutes(userSessionExpiration!.AccessToken),
            RefreshDeadline = utcNow.AddMinutes(userSessionExpiration.RefreshToken)
        };

        userSession.AccessToken = tokenGenerater.Generate(username, userSession.Deadline);
        userSession.RefreshToken = tokenGenerater.Generate(username, userSession.RefreshDeadline);

        user.Sessions.Add(userSession);
        await dbContext.UpdateAsync(user);

        return objectMapper.Map<ViewUserSessionModel>(userSession);
    }
}