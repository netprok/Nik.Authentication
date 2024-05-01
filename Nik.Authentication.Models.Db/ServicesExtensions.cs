namespace Nik.Authentication.Models.Db;

public static class ServicesExtensions
{
    private const string AuthenticationConnectionSettingsName = "AuthenticationMongoDB";
    private const string AuthenticationDbSettingsName = "DatabaseSettings:AuthenticationDatabaseName";

    public static IServiceCollection ConfigureNikAuthenticationMongoDb(this IServiceCollection services)
    {
        var mongoConnectionString = Context.Configuration.GetConnectionString(AuthenticationConnectionSettingsName);
        var databaseName = Context.Configuration[AuthenticationDbSettingsName];

        services.AddSingleton(new AuthenticationDbContext(mongoConnectionString!, databaseName!));

        return services;
    }
}