namespace NiC.Authentication.Models.Dtos;

public sealed class CompleteResetPasswordModel
{
    public string Username { get; set; } = string.Empty;

    public string ValidationKey { get; set; } = string.Empty;

    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[# ?!@$%^&*+/-]).{8,}$", ErrorMessage = "Password is not strong")]
    public string NewPassword { get; set; } = string.Empty;

    public string MachineName { get; set; } = string.Empty;
}