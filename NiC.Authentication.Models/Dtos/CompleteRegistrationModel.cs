namespace NiC.Authentication.Models.Dtos;

public sealed class CompleteRegistrationModel
{
    public string Username { get; set; } = string.Empty;

    public string ValidationKey { get; set; } = string.Empty;

    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[# ?!@$%^&*+/-]).{8,}$", ErrorMessage = "Password is not strong")]
    public string Password { get; set; } = string.Empty;

    [MinLength(2)]
    public string DisplayName { get; set; } = string.Empty;

    public string MachineName { get; set; } = string.Empty;
}