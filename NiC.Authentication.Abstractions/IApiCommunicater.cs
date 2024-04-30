namespace NiC.Authentication.Abstractions;

public interface IApiCommunicater
{
    Task<HttpResponseMessage> PostAsJsonAsync<TValue>(string configKey, string action, TValue value);

    Task<HttpResponseMessage> GetAsync(string configKey, string action);
}