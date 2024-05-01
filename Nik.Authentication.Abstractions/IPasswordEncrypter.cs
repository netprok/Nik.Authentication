namespace Nik.Authentication.Abstractions;

public interface IPasswordEncrypter
{
    bool SetUserPassword(User user, string password);
}