namespace Nik.Authentication;

internal sealed class Registerer(
    IUserSessionWriter userSessionWriter,
    IAuthenticationValidater authenticationValidater,
    IValidationKeyGenerater validationKeyGenerater,
    IPasswordEncrypter passwordEncrypter,
    ILogger<Registerer> logger,
    AuthenticationDbContext dbContext) : IRegisterer
{
    private const int DeadlineMinutes = 5;

    public async Task<ViewRegistrationModel?> RegisterAsync(RegisterModel registerModel)
    {
        if (!await authenticationValidater.IsRegistrationValidAsync(registerModel))
        {
            return null;
        }

        var utcNow = DateTime.UtcNow;
        Registration registration = new()
        {
            Username = registerModel.Username,
            CreateTime = utcNow,
            Deadline = utcNow.AddMinutes(DeadlineMinutes),
            ValidationStatus = (short)EValidationStatus.Created,
            ValidationKey = validationKeyGenerater.Generate(),
            ValidationMethod = registerModel.ValidationMethod,
        };
        await dbContext.Registrations.InsertOneAsync(registration);

        logger.LogInformation($"The registration was saved but not completed yet, of the user: {registerModel.Username}.");
        return new()
        {
            Username = registerModel.Username,
            ValidationKey = registration.ValidationKey
        };
    }

    public async Task<ViewUserSessionModel?> CompleteRegistrationAsync(CompleteRegistrationModel model)
    {
        var completeRegistrationResult = await authenticationValidater.ValidateCompleteRegistrationAsync(model);
        if (completeRegistrationResult is null)
        {
            return null;
        }

        User user = new()
        {
            Username = model.Username,
            Email = completeRegistrationResult.Email,
            Phone = completeRegistrationResult.Phone,
            CreateTime = DateTime.UtcNow,
            DisplayName = model.DisplayName,
            IsActive = true,
        };

        passwordEncrypter.SetUserPassword(user, model.Password);
        await dbContext.Users.InsertOneAsync(user);

        var filterOldRegistrations = Builders<Registration>.Filter.Lt(registration => registration.Deadline, DateTime.UtcNow);
        await dbContext.Registrations.DeleteManyAsync(filterOldRegistrations);

        // Updating registration status
        var filter = Builders<Registration>.Filter.Eq(registration => registration.Username, model.Username) & Builders<Registration>.Filter.Eq(registration => registration.ValidationStatus, EValidationStatus.Created);
        var update = Builders<Registration>.Update.Set(registration => registration.ValidationStatus, EValidationStatus.Completed);
        var updateOptions = new UpdateOptions { IsUpsert = false };
        var updateResult = await dbContext.Registrations.UpdateOneAsync(filter, update, updateOptions);

        logger.LogInformation($"Registration of the user: {model.Username} completed successfully.");
        return await userSessionWriter.GenerateAsync(user.Username, model.MachineName);
    }
}