[assembly: InternalsVisibleTo("NiC.Authentication.Seeder")]
[assembly: InternalsVisibleTo("NiC.Authentication.Api")]
[assembly: InternalsVisibleTo("NiC.Authentication.UnitTests")]

namespace NiC.Authentication.Models.Db;

internal static class ServicesExtensions
{
    private const string AuthenticationConnectionSettingsName = "AuthenticationMongoDB";
    private const string AuthenticationDbSettingsName = "DatabaseSettings:DatabaseName";

    public static IServiceCollection ConfigureAuthenticationMongoDb(this IServiceCollection services)
    {
        var mongoConnectionString = Context.Configuration.GetConnectionString(AuthenticationConnectionSettingsName);
        var databaseName = Context.Configuration[AuthenticationDbSettingsName];

        services.AddSingleton(new AuthenticationDbContext(mongoConnectionString!, databaseName!));

        return services;
    }
}