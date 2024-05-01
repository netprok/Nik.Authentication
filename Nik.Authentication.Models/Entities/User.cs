namespace Nik.Authentication.Models.Entities;

public sealed class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public string Username { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public byte[] Password { get; set; } = [];

    public byte[] Salt { get; set; } = [];

    public DateTime CreateTime { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = false;

    public List<UserSession> Sessions { get; set; } = [];

    public List<ResetPasswordRequest> ResetPasswordRequests { get; set; } = [];
}