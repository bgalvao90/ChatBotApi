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

        public AtendimentoService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Atendimento?> ObterPorIdAsync(int id)
        {
            return await _uow.AtendimentoRepository.GetAsync(a => a.Id == id);
        }

        public async Task FinalizarAtendimentoAsync(int id)
        {
            var atendimento = await _uow.AtendimentoRepository.GetAsync(a => a.Id == id);

            if (atendimento == null)
                throw new Exception("Atendimento não encontrado.");

            atendimento.Status = AtendimentoStatus.Concluido;
            await _uow.CommitAsync();
        }
    }
}
