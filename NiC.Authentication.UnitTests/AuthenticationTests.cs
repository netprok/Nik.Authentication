#nullable disable

namespace NiC.Authentication.UnitTests;

public class AuthenticationTests
{
    private IAuthenticater _authenticater;
    private IRegisterer _registerer;
    private IPasswordChanger _passwordChanger;

    public AuthenticationTests()
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder([]);

        builder.Services
            .InitContext()
            .AddNikCommon()
            .AddNikSecurity()
            .ConfigureAuthenticationMongoDb()
            .AddAuthenticationAuthentication();
        var app = builder.Build();

        _authenticater = app.Services.GetService<IAuthenticater>();
        _registerer = app.Services.GetService<IRegisterer>();
        _passwordChanger = app.Services.GetService<IPasswordChanger>();
    }

    [Fact]
    public async Task TestAuthentication()
    {
        const string invalidUsername = "user";
        string username = "e" + Guid.NewGuid().ToString("N")[..10] + "@gmail.com";
        const string invalidPassword = "simple";
        const string password = "p1A2-B1 d";
        const string wrongPassword = "wrong password";
        const string newPassword = "Str0Ng-p4$";
        const string weakPassword = "weak";

        // login with invalid username and password
        LoginModel loginModel = new()
        {
            Username = invalidUsername,
            Password = invalidPassword,
            MachineName = Environment.MachineName,
        };
        (await _authenticater.LoginAsync(loginModel)).Should().BeNull();

        // register with invalid email
        RegisterModel registerModel = new()
        {
            Username = invalidUsername,
            ValidationMethod = Models.Enums.EValidationMethod.Email,
        };
        (await _registerer.RegisterAsync(registerModel)).Should().BeNull();

        // register
        registerModel.Username = username;
        var viewRegistrationModel = await _registerer.RegisterAsync(registerModel);
        viewRegistrationModel.Should().NotBeNull();

        // complete registration
        CompleteRegistrationModel completeRegistrationModel = new()
        {
            Username = username,
            ValidationKey = viewRegistrationModel.ValidationKey,
            DisplayName = "TestUser",
            Password = password,
            MachineName = Environment.MachineName,
        };
        (await _registerer.CompleteRegistrationAsync(completeRegistrationModel)).Should().NotBeNull();

        // valid login
        loginModel = new()
        {
            Username = username,
            Password = password,
            MachineName = Environment.MachineName,
        };
        var viewUserSessionModel = await _authenticater.LoginAsync(loginModel);
        viewUserSessionModel.Should().NotBeNull();

        // login with token
        LoginWithTokenModel loginWithTokenModel = new()
        {
            Token = viewUserSessionModel.AccessToken,
            MachineName = Environment.MachineName,
        };
        (await _authenticater.LoginWithTokenAsync(loginWithTokenModel)).Should().Be(completeRegistrationModel.Username);

        // refresh token
        RefreshTokenModel refreshTokenModel = new()
        {
            MachineName = Environment.MachineName,
            Token = viewUserSessionModel.RefreshToken
        };
        viewUserSessionModel = await _authenticater.RefreshTokenAsync(refreshTokenModel);
        viewUserSessionModel.Should().NotBeNull();

        // login with refreshed token
        loginWithTokenModel = new()
        {
            Token = viewUserSessionModel.AccessToken,
            MachineName = Environment.MachineName,
        };
        (await _authenticater.LoginWithTokenAsync(loginWithTokenModel)).Should().Be(completeRegistrationModel.Username);

        // logout
        LogoutModel logoutModel = new()
        {
            Token = viewUserSessionModel.AccessToken,
            MachineName = loginModel.MachineName,
        };
        (await _authenticater.LogoutAsync(logoutModel)).Should().BeTrue();

        // login with invalid token
        (await _authenticater.LoginWithTokenAsync(loginWithTokenModel)).Should().Be(string.Empty);

        // change password with wrong old password
        ChangePasswordModel changePasswordModel = new()
        {
            MachineName = Environment.MachineName,
            Username = username,
            OldPassword = wrongPassword,
            NewPassword = weakPassword
        };
        (await _passwordChanger.ChangePasswordAsync(changePasswordModel)).Should().BeNull();

        // change password with week new one
        changePasswordModel.OldPassword = password;
        (await _passwordChanger.ChangePasswordAsync(changePasswordModel)).Should().BeNull();

        // change password with strong new one
        changePasswordModel.NewPassword = newPassword;
        (await _passwordChanger.ChangePasswordAsync(changePasswordModel)).Should().NotBeNull();

        // invalid login with the old password
        loginModel = new()
        {
            Username = username,
            Password = password,
            MachineName = Environment.MachineName,
        };
        (await _authenticater.LoginAsync(loginModel)).Should().BeNull();

        // valid login with new password
        loginModel.Password = newPassword;
        (await _authenticater.LoginAsync(loginModel)).Should().NotBeNull();

        // login with token
        loginWithTokenModel.Token = viewUserSessionModel.AccessToken;
        (await _authenticater.LoginWithTokenAsync(loginWithTokenModel)).Should().Be(completeRegistrationModel.Username);

        // reset password
        ResetPasswordRequestModel resetPasswordModel = new()
        {
            MachineName = Environment.MachineName,
            Username = username,
            ValidationMethod = Models.Enums.EValidationMethod.Email,
        };
        ViewResetPasswordRequestModel viewResetPasswordRequestModel = await _passwordChanger.RequestResetPasswordAsync(resetPasswordModel);
        viewResetPasswordRequestModel.Should().NotBeNull();

        // complete reset password
        CompleteResetPasswordModel completeResetPasswordModel = new()
        {
            MachineName = Environment.MachineName,
            NewPassword = "§st1o56Pa$+",
            Username = username,
            ValidationKey = viewResetPasswordRequestModel.ValidationKey,
        };
        viewUserSessionModel = await _passwordChanger.CompleteResetPasswordAsync(completeResetPasswordModel);
        viewUserSessionModel.Should().NotBeNull();

        // login with new token
        loginWithTokenModel.Token = viewUserSessionModel.AccessToken;
        (await _authenticater.LoginWithTokenAsync(loginWithTokenModel)).Should().Be(completeRegistrationModel.Username);
    }
}