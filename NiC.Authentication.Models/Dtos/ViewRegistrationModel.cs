namespace NiC.Authentication.Models.Dtos;

public sealed class ViewRegistrationModel
{
    public string Username { get; set; } = string.Empty;

    public string ValidationKey { get; set; } = string.Empty;
}