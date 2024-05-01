namespace Nik.Authentication;

public sealed class ValidationKeyGenerater : IValidationKeyGenerater
{
    public string Generate() => Guid.NewGuid().ToString("N")[..20];
}