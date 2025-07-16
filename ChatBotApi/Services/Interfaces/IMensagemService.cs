using ChatBotApi.Models;

namespace ChatBotApi.Services.Interfaces
{
    public interface IMensagemService
    {
        Task EnviarMensagemParaAtendimentoAsync(int atendimentoId, Mensagem mensagem);
    }
}
