using ChatBotApi.DTOs;
using ChatBotApi.Models;
using ChatBotApi.Models.Enums;

namespace ChatBotApi.Services.Interfaces
{
    public interface IAtendimentoService
    {
        Task<Atendimento?> ObterPorIdAsync(int id);
        Task<bool> StatusAtendimentoAsync(int id, AtendimentoStatus status);
        Task<bool> ResponderClienteAsync(RespostaAtendenteDto respostaDto);

    }
}
