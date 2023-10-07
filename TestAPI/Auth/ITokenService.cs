namespace TestAPI.Auth
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, APIUserDto user);
    }
}
