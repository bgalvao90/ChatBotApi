using ChatBotApi.Models;

namespace ChatBotApi.Repositories.Interfaces
{
    public interface IMensagemRepository : IRepository<Mensagem>
    {
        Task<IEnumerable<Mensagem>> ObterPorAtendimentoIdAsync(int atendimentoId);
    }
}
