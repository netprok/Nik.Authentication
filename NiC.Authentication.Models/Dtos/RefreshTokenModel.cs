namespace NiC.Authentication.Models.Dtos;

public sealed class RefreshTokenModel
{
    public string Token { get; set; } = string.Empty;

    public string MachineName { get; set; } = string.Empty;
}