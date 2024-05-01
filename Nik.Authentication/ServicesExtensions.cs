namespace Nik.Authentication;

public static class ServicesExtensions
{
    public static IServiceCollection AddNiCAuthentication(this IServiceCollection services)
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