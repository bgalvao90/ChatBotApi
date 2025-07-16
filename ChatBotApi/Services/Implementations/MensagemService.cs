using ChatBotApi.Models;
using ChatBotApi.Repositories.Interfaces;
using ChatBotApi.Services.Interfaces;

namespace ChatBotApi.Services.Implementations
{
    public class MensagemService : IMensagemService
    {
        private readonly IUnitOfWork _uow;

        public MensagemService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task EnviarMensagemParaAtendimentoAsync(int atendimentoId, Mensagem mensagem)
        {
            mensagem.AtendimentoId = atendimentoId;
            await _uow.MensagemRepository.CreateAsync(mensagem);
            await _uow.CommitAsync();
        }
    }
}
