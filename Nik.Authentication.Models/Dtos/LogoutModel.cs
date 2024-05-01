namespace Nik.Authentication.Models.Dtos;

public sealed class LogoutModel
{
    public string Token { get; set; } = string.Empty;

    public string MachineName { get; set; } = string.Empty;
}