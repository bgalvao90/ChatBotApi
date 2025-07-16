using ChatBotApi.Models;
using ChatBotApi.Repositories.Interfaces;
using ChatBotApi.Services.Interfaces;

namespace ChatBotApi.Services.Implementations
{
    public class DistribuidorService : IDistribuidorService
    {
        private readonly IAtendenteRepository _repo;
        public DistribuidorService(IAtendenteRepository repo)
        {
            _repo = repo;
        }

        public async Task<Atendente?> ObterAtendenteDisponivelAsync()
        {
            return await _repo.ObterAtendenteComMenorAtendimentosAsync();
        }
    }
}
