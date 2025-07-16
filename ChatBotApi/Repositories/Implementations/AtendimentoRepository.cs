using ChatBotApi.Context;
using ChatBotApi.Models;
using ChatBotApi.Models.Enums;
using ChatBotApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatBotApi.Repositories.Implementations
{
    public class AtendimentoRepository : Repository<Atendimento>, IAtendimentoRepository
    {
        private readonly AppDbContext _context;

        public AtendimentoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Atendimento?> BuscarAtivoPorUsuarioExterno(string idUsuarioExterno)
        {
            return await _context.Atendimentos
                .Include(a => a.Mensagens)
                .FirstOrDefaultAsync(a => a.IdUsuarioExterno == idUsuarioExterno && a.Status != AtendimentoStatus.Concluido);
        }
    }
}
