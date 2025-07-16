using ChatBotApi.Context;
using ChatBotApi.Models;
using ChatBotApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatBotApi.Repositories.Implementations
{
    public class MensagemRepository : Repository<Mensagem>, IMensagemRepository
    {
        public MensagemRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Mensagem>> ObterPorAtendimentoIdAsync(int atendimentoId)
        {
            return await _context.Mensagens!
               .Where(m => m.AtendimentoId == atendimentoId)
               .OrderBy(m => m.DataHora)
               .ToListAsync();
        }
    }
}
