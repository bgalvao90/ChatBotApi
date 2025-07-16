using AutoMapper;
using ChatBotApi.DTOs;
using ChatBotApi.Models;
using ChatBotApi.Models.Enums;
using ChatBotApi.Repositories.Interfaces;
using ChatBotApi.Services.Interfaces;

namespace ChatBotApi.Services.Implementations
{
    public class AtendimentoService : IAtendimentoService
    {
        private readonly IUnitOfWork _uow;
        private readonly ICanalService _canalService;

        public AtendimentoService(IUnitOfWork uow, ICanalService canalService)
        {
            _uow = uow;
            _canalService = canalService;
        }

        public async Task<bool> ResponderClienteAsync(RespostaAtendenteDto respostaDto)
        {
            var atendimento = await _uow.AtendimentoRepository.GetAsync(a => a.Id == respostaDto.AtendimentoId);

            if (atendimento == null)
                return false;

            var mensagemResposta = new Mensagem
            {
                AtendimentoId = atendimento.Id,
                Conteudo = respostaDto.Conteudo,
                EnviadoPor = respostaDto.EnviadoPor,
                DataHora = respostaDto.DataHora,
                EnviadaPorAtendente = true,
                Canal = atendimento.Canal,
                IdUsuarioExterno = atendimento.IdUsuarioExterno
            };

            await _uow.MensagemRepository.CreateAsync(mensagemResposta);
            await _uow.CommitAsync();

            // Enviar mensagem pelo canal correto
            await _canalService.EnviarMensagemParaUsuario(
            atendimento.Canal,
            atendimento.IdUsuarioExterno,
            mensagemResposta.Conteudo
        );

            return true;
        }


        public async Task<Atendimento?> ObterPorIdAsync(int id)
        {
            return await _uow.AtendimentoRepository.GetAsync(a => a.Id == id);
        }

        public async Task<bool> StatusAtendimentoAsync(int id, AtendimentoStatus status)
        {
            var atendimento = await _uow.AtendimentoRepository.GetAsync(a => a.Id == id);

            if (atendimento == null)
                return false;

            atendimento.Status = status;
            await _uow.CommitAsync();
            return true;
        }
    }
}
