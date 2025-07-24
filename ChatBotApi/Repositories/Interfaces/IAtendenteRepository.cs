using ChatBotApi.Models;

namespace ChatBotApi.Repositories.Interfaces
{
    public interface IAtendenteRepository : IRepository<Atendente>
    {
        Task<Atendente?> ObterAtendenteComMenorAtendimentosAsync(string categoria);
    }
}
