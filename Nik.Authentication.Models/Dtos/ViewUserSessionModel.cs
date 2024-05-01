namespace Nik.Authentication.Models.Dtos;

public sealed class ViewUserSessionModel
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime Deadline { get; set; } = DateTime.MinValue;

    public DateTime RefreshDeadline { get; set; } = DateTime.MinValue;
}