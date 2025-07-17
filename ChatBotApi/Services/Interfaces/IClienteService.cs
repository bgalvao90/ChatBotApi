using ChatBotApi.Models;

namespace ChatBotApi.Services.Interfaces
{
    public interface IClienteService
    {
        Task<Cliente?> ObterPorUserModelIdAsync(int UserModelId);
    }
}
