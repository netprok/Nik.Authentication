namespace NiC.Authentication.Abstractions;

public interface IAuthenticationValidater
{
    Task<CompleteRegistrationResult?> ValidateCompleteRegistrationAsync(CompleteRegistrationModel completeRegistrationModel);

    Task<bool> IsLoginValidAsync(LoginModel loginModel);

    bool IsPasswordStrong(string username, string password);

    Task<bool> IsRegistrationValidAsync(RegisterModel registerModel);

    Task<bool> IsRequestResetPasswordValidAsync(ResetPasswordRequestModel resetPasswordRequestModel);

    Task<bool> IsCompleteResetPasswordValidAsync(CompleteResetPasswordModel completeResetPasswordModel);
}