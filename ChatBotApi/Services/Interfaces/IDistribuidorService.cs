using ChatBotApi.Models;

namespace ChatBotApi.Services.Interfaces
{
    public interface IDistribuidorService
    {
        Task<Atendente?> ObterAtendenteDisponivelAsync();
    }
}
