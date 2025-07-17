
using ChatBotApi.Models;
using ChatBotApi.Repositories.Interfaces;
using ChatBotApi.Repositories.Interfaces;
using ChatBotApi.Services.Interfaces;

namespace ChatBotApi.Services.Implementations
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _uow;

        public ClienteService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Cliente?> ObterPorUserModelIdAsync(int UserModelId)
        {
            return await _uow.ClienteRepository.GetAsync(c => c.UserModelId == UserModelId);
        }
    }
}
