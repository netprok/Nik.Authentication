namespace Nik.Authentication.Models.Dtos;

public sealed class ViewResetPasswordRequestModel
{
    public string Username { get; set; } = string.Empty;

    public string ValidationKey { get; set; } = string.Empty;
}