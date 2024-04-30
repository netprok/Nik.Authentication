namespace NiC.Authentication;

internal sealed class AuthenticationValidater(
    IAesConfigrationProvider aesConfigrationProvider,
    IAesEncryptor aesEncryptor,
    IUserLoader userLoader,
    ILogger<AuthenticationValidater> logger,
    AuthenticationDbContext dbContext) : IAuthenticationValidater
{
    public async Task<CompleteRegistrationResult?> ValidateCompleteRegistrationAsync(CompleteRegistrationModel model)
    {
        model.DisplayName = model.DisplayName.Trim();
        if (model.DisplayName.Length < 2)
        {
            logger.LogInformation($"Invalid complete registration of the user: {model.Username}. Invalid display name.");
            return null;
        }

        model.MachineName = model.MachineName.Trim();
        if (string.IsNullOrEmpty(model.MachineName))
        {
            logger.LogInformation($"Invalid complete registration of the user: {model.Username}. Invalid machine name");
            return null;
        }

        if (!IsPasswordStrong(model.Username, model.Password))
        {
            return null;
        }

        var filter = Builders<Registration>.Filter.And(
            Builders<Registration>.Filter.Eq(registration => registration.Username, model.Username),
            Builders<Registration>.Filter.Eq(registration => registration.ValidationStatus, EValidationStatus.Created),
            Builders<Registration>.Filter.Gt(registration => registration.Deadline, DateTime.UtcNow),
            Builders<Registration>.Filter.Eq(registration => registration.ValidationKey, model.ValidationKey)
        );

        var registrations = await dbContext.Registrations.Find(filter).ToListAsync();

        if (registrations.Count == 0)
        {
            logger.LogInformation($"Invalid complete registration of the user: {model.Username}. Registration not found or dead.");
            return null;
        }

        var registration = registrations.First();
        registration.ValidationStatus = EValidationStatus.Completed;

        return new()
        {
            Username = registration.Username,
            Email = registration.ValidationMethod == EValidationMethod.Email ? registration.Username : string.Empty,
            Phone = registration.ValidationMethod == EValidationMethod.Sms ? registration.Username : string.Empty,
        };
    }

    public bool IsPasswordStrong(string username, string password)
    {
        Regex validatePasswordRegex = new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[# ?!@$%^&*+/-]).{8,}$");
        if (!validatePasswordRegex.IsMatch(password))
        {
            logger.LogInformation($"Tried to set week password for the user: {username}");
            return false;
        }

        return true;
    }

    public async Task<bool> IsLoginValidAsync(LoginModel loginModel)
    {
        loginModel.MachineName = loginModel.MachineName.Trim();
        if (string.IsNullOrEmpty(loginModel.MachineName))
        {
            logger.LogWarning($"Login failed for user: {loginModel.Username}. machine name not given.");
            return false;
        }

        var user = await userLoader.LoadByNameAsync(loginModel.Username);
        if (user == default)
        {
            logger.LogWarning($"Tried to login with invalid user: {loginModel.Username} on machine: {loginModel.MachineName}");
            return false;
        }

        var aesConfig = aesConfigrationProvider.Provide();

        string decryptedPasswordSalt = aesEncryptor.Decrypt(user.Password, aesConfig);
        string salt = aesEncryptor.Decrypt(user.Salt, aesConfig);
        var decryptedPassword = decryptedPasswordSalt[..^salt.Length];

        bool passwordCorrect = decryptedPassword == loginModel.Password;
        if (!passwordCorrect)
        {
            logger.LogWarning($"Tried to login with user: {loginModel.Username} on machine: {loginModel.MachineName}. Password not correct.");
        }
        return passwordCorrect;
    }

    public async Task<bool> IsRegistrationValidAsync(RegisterModel model)
    {
        model.Username = model.Username.Trim();

        if (model.ValidationMethod != EValidationMethod.Email) // todo: implement registration with app and phone
        {
            logger.LogInformation($"Invalid registration method: {model.ValidationMethod}.");
            return false;
        }

        if (model.ValidationMethod == EValidationMethod.Email && !IsEmailValid(model.Username))
        {
            logger.LogInformation($"Invalid username: {model.Username}.");
            return false;
        }

        var exists = await userLoader.UserExistsAsync(model.Username);
        if (exists)
        {
            logger.LogInformation($"Username already registered: {model.Username}.");
            return false;
        }

        return true;
    }

    private bool IsEmailValid(string email)
    {
        try
        {
            MailAddress _ = new MailAddress(email);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public async Task<bool> IsRequestResetPasswordValidAsync(ResetPasswordRequestModel model)
    {
        if (!IsEmailValid(model.Username))
        {
            logger.LogWarning($"Tried to request reset password of an invalid username: {model.Username}.");
            return false;
        }
        var user = await userLoader.LoadByNameAsync(model.Username);
        if (user is null)
        {
            logger.LogWarning($"Tried to request reset password of a non existing username: {model.Username}.");
            return false;
        }

        return true;
    }

    public async Task<bool> IsCompleteResetPasswordValidAsync(CompleteResetPasswordModel model)
    {
        model.MachineName = model.MachineName.Trim();
        if (string.IsNullOrEmpty(model.MachineName))
        {
            logger.LogInformation($"Invalid complete reset password of the user: {model.Username}. Invalid machine name");
            return false;
        }
        if (!IsPasswordStrong(model.Username, model.NewPassword))
        {
            return false;
        }

        var user = await userLoader.LoadByNameAsync(model.Username);
        var isValid = user.ResetPasswordRequests.Any(request => request.ValidationStatus == EValidationStatus.Created && request.Deadline > DateTime.UtcNow && request.ValidationKey == model.ValidationKey);

        if (!isValid)
        {
            logger.LogInformation($"Invalid complete reset password of the user: {model.Username}. Request not found or dead.");
            return false;
        }

        return true;
    }
}