namespace NiC.Authentication.Abstractions;

public interface IUserSessionWriter
{
    Task DeleteAsync(string username, string machineName, string machineName1);

    Task<ViewUserSessionModel> GenerateAsync(string username, string machineName);
}