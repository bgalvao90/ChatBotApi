using ChatBotApi.DTOs;

namespace ChatBotApi.Services.Interfaces
{
    public interface IAtendimentoService
    {
        Task CriarOuEncaminharAtendimentoAsync(MensagemEntradaDto dto);
        Task EnviarRespostaDoAtendenteAsync(RespostaAtendenteDto dto);
    }
}
