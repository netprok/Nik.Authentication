namespace NiC.Authentication.Abstractions;

public interface IPasswordChanger
{
    Task<ViewUserSessionModel?> ChangePasswordAsync(ChangePasswordModel changePasswordModel);

    Task<ViewResetPasswordRequestModel?> RequestResetPasswordAsync(ResetPasswordRequestModel resetPasswordRequestModel);

    Task<ViewUserSessionModel?> CompleteResetPasswordAsync(CompleteResetPasswordModel completeResetPasswordModel);
}