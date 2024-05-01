namespace Nik.Authentication.Models.Dtos;

public sealed class ResetPasswordRequestModel
{
    public string Username { get; set; } = string.Empty;

    public string MachineName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public EValidationMethod ValidationMethod { get; set; }
}