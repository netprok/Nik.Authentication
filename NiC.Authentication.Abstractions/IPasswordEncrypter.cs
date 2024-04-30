namespace NiC.Authentication.Abstractions;

public interface IPasswordEncrypter
{
    bool SetUserPassword(User user, string password);
}