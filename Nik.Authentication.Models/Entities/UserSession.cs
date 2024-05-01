namespace Nik.Authentication.Models.Entities;

public sealed class UserSession
{
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;

    public string MachineName { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime Deadline { get; set; } = DateTime.MinValue;

    public DateTime RefreshDeadline { get; set; } = DateTime.MinValue;
}