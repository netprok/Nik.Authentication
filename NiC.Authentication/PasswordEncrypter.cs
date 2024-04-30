namespace NiC.Authentication;

public sealed class PasswordEncrypter(
    IAesConfigrationProvider aesConfigrationProvider,
        IAesEncryptor aesEncryptor
    ) : IPasswordEncrypter
{
    public bool SetUserPassword(User user, string password)
    {
        var (encryptedPassword, encryptedSalt) = Encrypt(password);
        if (encryptedPassword is null || encryptedSalt is null)
        {
            return false;
        }
        user.Password = encryptedPassword;
        user.Salt = encryptedSalt;

        return true;
    }

    private (byte[]? encryptedPassword, byte[]? encryptedSalt) Encrypt(string password)
    {
        var aesConfig = aesConfigrationProvider.Provide();

        string salt = Guid.NewGuid().ToString("N")[..new Random().Next(8, 12)];
        return (aesEncryptor.Encrypt(password + salt, aesConfig),
                aesEncryptor.Encrypt(salt, aesConfig));
    }
}