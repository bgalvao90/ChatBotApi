using System.Linq.Expressions;
using ChatBotApi.Models;

namespace ChatBotApi.Repositories.Interfaces
{
    public interface IAtendimentoRepository : IRepository<Atendimento>
    {
        Task<Atendimento?> BuscarAtivoPorUsuarioExterno(string idUsuarioExterno);
        IQueryable<Atendimento> GetQueryable(Expression<Func<Atendimento, bool>> filter = null);

    }
}
