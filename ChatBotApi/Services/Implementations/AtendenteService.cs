using ChatBotApi.Models;
using ChatBotApi.Models.Enums;
using ChatBotApi.Repositories.Interfaces;
using ChatBotApi.Services.Interfaces;

namespace ChatBotApi.Services.Implementations
{
    public class AtendenteService : IAtendenteService
    {
        private readonly IUnitOfWork _uow;

        public AtendenteService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<Atendente>> ObterDisponiveisAsync()
        {
            var atendente = await _uow.AtendenteRepository.ObterAtendenteComMenorAtendimentosAsync();
            if (atendente == null)
            {
                return Enumerable.Empty<Atendente>();
            }
            return new List<Atendente> { atendente };
        }

        public async Task AtualizarStatusAsync(int id, AtendenteStatus status)
        {
            var atendente = await _uow.AtendenteRepository.GetAsync(a => a.Id == id);
            if (atendente == null)
                throw new Exception("Atendente não encontrado.");

            atendente.Status = status;
            await _uow.CommitAsync();
        }
        public async Task<Atendente?> ObterPorUserModelIdAsync(int UserModelId)
        {
            return await _uow.AtendenteRepository.GetAsync(c => c.UserModelId == UserModelId);
        }

        public async Task<Atendente?> ObterPorId(int id)
        {
            return await _uow.AtendenteRepository.GetAsync(a => a.Id == id);
        }
    }
}
