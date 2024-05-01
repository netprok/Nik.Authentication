namespace Nik.Authentication.Models.Entities;

public sealed class ResetPasswordRequest
{
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;

    public EValidationStatus ValidationStatus { get; set; }

    public EValidationMethod ValidationMethod { get; set; }

    public string ValidationKey { get; set; } = string.Empty;

    public DateTime Deadline { get; set; } = DateTime.MinValue;
}