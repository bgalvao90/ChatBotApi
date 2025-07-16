using ChatBotApi.DTOs;
using ChatBotApi.Models;

namespace ChatBotApi.Services.Interfaces
{
    public interface IAtendimentoService
    {
        Task<Atendimento?> ObterPorIdAsync(int id);
        Task FinalizarAtendimentoAsync(int id);
    }
}
