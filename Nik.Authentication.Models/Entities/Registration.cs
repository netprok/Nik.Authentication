namespace Nik.Authentication.Models.Entities;

public sealed class Registration
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public DateTime CreateTime { get; set; } = DateTime.UtcNow;

    public string Username { get; set; } = string.Empty;

    public EValidationStatus ValidationStatus { get; set; }

    public EValidationMethod ValidationMethod { get; set; }

    public string ValidationKey { get; set; } = string.Empty;

    public DateTime Deadline { get; set; } = DateTime.MinValue;
}