using ChatBotApi;

namespace ChatBotApi { 

public interface ITokenService
{
    string GerarToken(string key, string issuer, string audience, UserModel user);
}

}