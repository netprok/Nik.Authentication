[assembly: InternalsVisibleTo("NiC.Authentication.Seeder")]
[assembly: InternalsVisibleTo("NiC.Authentication.Authentication.Api")]
[assembly: InternalsVisibleTo("NiC.Authentication.Authorization.Api")]
[assembly: InternalsVisibleTo("NiC.Authentication.UnitTests")]

namespace NiC.Authentication;

internal static class ServicesExtensions
{
    public static IServiceCollection AddAuthenticationAuthentication(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticater, Authenticater>();
        services.AddScoped<IRegisterer, Registerer>();
        services.AddScoped<IValidationKeyGenerater, ValidationKeyGenerater>();
        services.AddScoped<IPasswordChanger, PasswordChanger>();

        services.AddScoped<IUserLoader, UserLoader>();
        services.AddScoped<IAuthenticationValidater, AuthenticationValidater>();
        services.AddScoped<IUserSessionValidater, UserSessionValidator>();
        services.AddScoped<IUserSessionWriter, UserSessionWriter>();
        services.AddScoped<IPasswordEncrypter, PasswordEncrypter>();

        return services;
    }
}