namespace Nik.Authentication.Models.Dtos;

public sealed class ChangePasswordModel
{
    public string Username { get; set; } = string.Empty;

    public string OldPassword { get; set; } = string.Empty;

    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[# ?!@$%^&*+/-]).{8,}$", ErrorMessage = "Password is not strong")]
    public string NewPassword { get; set; } = string.Empty;

    public string MachineName { get; set; } = string.Empty;
}