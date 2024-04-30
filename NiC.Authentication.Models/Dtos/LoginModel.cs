namespace NiC.Authentication.Models.Dtos;

public sealed class LoginModel
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string MachineName { get; set; } = string.Empty;
}