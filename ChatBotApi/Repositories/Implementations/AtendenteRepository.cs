using ChatBotApi.Context;
using ChatBotApi.Models;
using ChatBotApi.Models.Enums;
using ChatBotApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatBotApi.Repositories.Implementations
{
    public class AtendenteRepository : Repository<Atendente>, IAtendenteRepository
    {
        public AtendenteRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Atendente?> ObterAtendenteComMenorAtendimentosAsync()
        {
            return await _context.Atendentes
                .Where(a => a.Disponivel && a.Status == AtendenteStatus.Online)
                .OrderBy(a => a.Atendimentos.Count())
                .FirstOrDefaultAsync();
        }
    }
}
