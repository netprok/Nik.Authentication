namespace Nik.Authentication.Models;

public sealed class UserSessionExpiration
{
    public double AccessToken { get; set; }
    public double RefreshToken { get; set; }
}