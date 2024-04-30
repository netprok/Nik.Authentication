using NiC.Authentication.Models.Dtos;

namespace NiC.Authentication.Abstractions;

public interface IRegisterer
{
    Task<ViewRegistrationModel?> RegisterAsync(RegisterModel registerModel);

    Task<ViewUserSessionModel?> CompleteRegistrationAsync(CompleteRegistrationModel completeRegisterModel);
}