namespace ChatBotApi.Services.Interfaces
{
    public interface ICanalService
    {
        Task EnviarMensagemParaUsuario(string canal, string idExterno, string mensagem);
    }
}
