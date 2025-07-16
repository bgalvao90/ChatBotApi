using ApiCatalogoMinimalApi.Models;

namespace ApiCatalogoMinimalApi.Services.Interfaces
{
    public interface ITokenService
    {
        string GerarToken(string key, string issuer, string audience, UserModel user);
    }
}
