namespace NiC.Authentication.Models.Dtos;

public sealed class LoginWithTokenModel
{
    public string Token { get; set; } = string.Empty;

    public string MachineName { get; set; } = string.Empty;
}