namespace Nik.Authentication.Models.Dtos;

public sealed class CompleteRegistrationResult
{
    public string Username { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }
}