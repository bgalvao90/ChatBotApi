using ChatBotApi.Models;
using ChatBotApi.Models.Enums;

namespace ChatBotApi.Services.Interfaces
{
    public interface IAtendenteService
    {
        Task<IEnumerable<Atendente>> ObterDisponiveisAsync();
        Task AtualizarStatusAsync(int id, AtendenteStatus status);
    }
}
