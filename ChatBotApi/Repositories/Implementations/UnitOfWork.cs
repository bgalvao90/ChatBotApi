using ChatBotApi.Context;
using ChatBotApi.Repositories.Interfaces;

namespace ChatBotApi.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private  IAtendimentoRepository _atendimentoRepository;
        private  IAtendenteRepository _atendenteRepository;
        private      IMensagemRepository _mensagemRepository;
        private AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IAtendenteRepository AtendenteRepository
        {
            get
            {
                return _atendenteRepository = _atendenteRepository ?? new AtendenteRepository(_context);
            }
        }
        public IAtendimentoRepository AtendimentoRepository
        {
            get
            {
                return _atendimentoRepository = _atendimentoRepository ?? new AtendimentoRepository(_context);
            }
        }
        public IMensagemRepository MensagemRepository
        {
            get
            {
                return _mensagemRepository = _mensagemRepository ?? new MensagemRepository(_context);
            }
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
