using ChatBotApi.Models;

namespace ChatBotApi.Repositories.Interfaces
{
    public interface IAtendimentoRepository : IRepository<Atendimento>
    {
        Task<Atendimento?> BuscarAtivoPorUsuarioExterno(string idUsuarioExterno);
    }
}
