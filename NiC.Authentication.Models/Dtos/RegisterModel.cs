namespace NiC.Authentication.Models.Dtos;

public sealed class RegisterModel
{
    public string Username { get; set; } = string.Empty;

    public EValidationMethod ValidationMethod { get; set; }
}