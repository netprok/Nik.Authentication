using Nik.Authentication.Models.Dtos;

namespace Nik.Authentication.Abstractions;

public interface IRegisterer
{
    Task<ViewRegistrationModel?> RegisterAsync(RegisterModel registerModel);

    Task<ViewUserSessionModel?> CompleteRegistrationAsync(CompleteRegistrationModel completeRegisterModel);
}