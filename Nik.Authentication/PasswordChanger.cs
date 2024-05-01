namespace Nik.Authentication;

internal sealed class PasswordChanger(
    IUserSessionWriter userSessionWriter,
    IAuthenticationValidater authenticationValidater,
    IValidationKeyGenerater validationKeyGenerater,
    IPasswordEncrypter passwordEncrypter,
    IUserLoader userLoader,
    ILogger<Authenticater> logger,
    AuthenticationDbContext dbContext) : IPasswordChanger
{
    private const int DeadlineMinutes = 5;

    public async Task<ViewUserSessionModel?> ChangePasswordAsync(ChangePasswordModel model)
    {
        if (!await authenticationValidater.IsLoginValidAsync(new()
        {
            Username = model.Username,
            Password = model.OldPassword,
            MachineName = model.MachineName
        }))
        {
            return null;
        }

        if (!authenticationValidater.IsPasswordStrong(model.Username, model.NewPassword))
        {
            return null;
        }

        var user = await userLoader.LoadByNameAsync(model.Username);

        if (!passwordEncrypter.SetUserPassword(user, model.NewPassword))
        {
            return null;
        }

        user.Sessions = [];
        await dbContext.UpdateAsync(user);

        logger.LogInformation($"Successfully changed password of the user: {model.Username} on machine: {model.MachineName}.");

        return await userSessionWriter.GenerateAsync(model.Username, model.MachineName);
    }

    public async Task<ViewResetPasswordRequestModel?> RequestResetPasswordAsync(ResetPasswordRequestModel model)
    {
        if (!await authenticationValidater.IsRequestResetPasswordValidAsync(model))
        {
            return null;
        }

        var user = await userLoader.LoadByNameAsync(model.Username);

        var utcNow = DateTime.UtcNow;
        ResetPasswordRequest resetPasswordRequest = new()
        {
            CreateTime = utcNow,
            Deadline = utcNow.AddMinutes(DeadlineMinutes),
            ValidationStatus = EValidationStatus.Created,
            ValidationKey = validationKeyGenerater.Generate(),
            ValidationMethod = model.ValidationMethod,
        };

        user.ResetPasswordRequests.Add(resetPasswordRequest);
        await dbContext.UpdateAsync(user);

        logger.LogInformation($"A reset password request done for the user: {model.Username}.");

        return new()
        {
            Username = model.Username,
            ValidationKey = resetPasswordRequest.ValidationKey,
        };
    }

    public async Task<ViewUserSessionModel?> CompleteResetPasswordAsync(CompleteResetPasswordModel model)
    {
        if (!await authenticationValidater.IsCompleteResetPasswordValidAsync(model))
        {
            return null;
        }

        var user = await userLoader.LoadByNameAsync(model.Username);

        if (!passwordEncrypter.SetUserPassword(user, model.NewPassword))
        {
            return null;
        }

        user.ResetPasswordRequests.RemoveAll(request => request.Deadline < DateTime.UtcNow);

        var request = user.ResetPasswordRequests.FirstOrDefault(request => request.ValidationStatus == EValidationStatus.Created);

        if (request is not null)
        {
            request.ValidationStatus = EValidationStatus.Completed;
        }

        user.Sessions = [];

        await dbContext.UpdateAsync(user);

        logger.LogInformation($"Reset password of the user: {model.Username} completed successfully.");
        return await userSessionWriter.GenerateAsync(model.Username, model.MachineName);
    }   
}