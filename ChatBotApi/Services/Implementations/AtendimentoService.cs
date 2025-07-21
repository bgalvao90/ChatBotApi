using AutoMapper;
using ChatBotApi.DTOs;
using ChatBotApi.Models;
using ChatBotApi.Models.Enums;
using ChatBotApi.Repositories.Interfaces;
using ChatBotApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

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
        public async Task<IEnumerable<Atendimento>> ObterAtendimentosAsync()
        {
            return await _uow.AtendimentoRepository.GetAllAsync();
        }
        public async Task<bool> ResponderClienteAsync(Mensagem respostaDto)
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
                Canal = atendimento.Canal,
                ImagemUrl = respostaDto.ImagemUrl,
                IdUsuarioExterno = atendimento.IdUsuarioExterno,
                EnviadaPorAtendente = false
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
        public async Task<bool> ResponderAtendenteAsync(Mensagem respostaDto)
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
                Canal = atendimento.Canal,
                ImagemUrl = respostaDto.ImagemUrl,
                IdUsuarioExterno = atendimento.IdUsuarioExterno,
                EnviadaPorAtendente = true
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
            return await _uow.AtendimentoRepository.GetQueryable(a => a.Id == id)
                .Include(a => a.Atendente)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Atendimento>> ListaMensagemFiltro(string conteudo)
        {
            var conteudoLower = conteudo.ToLower();

            var atendimentos = await _uow.AtendimentoRepository.GetAllAsync(a => a.Mensagens.Any
            (m => m.Conteudo != null && m.Conteudo.ToLower().Contains(conteudoLower)));

            return atendimentos.ToList();
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

        public async Task<bool> FinalizarAtendimentoAsync(int id)
        {
            try
            {
                var atendimento = await _uow.AtendimentoRepository.GetAsync(a => a.Id == id);


                if (atendimento == null)
                    return false;

                atendimento.Status = AtendimentoStatus.Concluido;
                await _uow.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter atendimento: " + ex.Message, ex);
            }
        }

        public async Task<List<Atendimento>> ListarPendentesAsync()
        {
            return await _uow.AtendimentoRepository
       .GetQueryable(a => a.Status != AtendimentoStatus.Concluido)
       .Include(a => a.Atendente)
       .ToListAsync();
        }

        public async Task<List<Atendimento>> ListarPendentesClienteAsync(int clienteId)
        {
            return await _uow.AtendimentoRepository
                .GetQueryable(a => a.Status != AtendimentoStatus.Concluido
                && a.ClienteId == clienteId)
                .Include(a => a.Atendente)
                .ToListAsync();
        }

        public async Task<bool> ResponderAtendimentoAsync(int id, int atendenteId, MensagemEntradaDto dto)
        {
            var atendimento = await _uow.AtendimentoRepository.GetAsync(a => a.Id == id);

            if (atendimento == null || atendimento.AtendenteId != atendenteId)
                return false;

            var mensagem = new Mensagem
            {
                AtendimentoId = atendimento.Id,
                Conteudo = dto.Conteudo,
                DataHora = dto.DataHora,
                Canal = atendimento.Canal,
                IdUsuarioExterno = atendimento.IdUsuarioExterno
            };

            await _uow.MensagemRepository.CreateAsync(mensagem);
            await _uow.CommitAsync();

            await _canalService.EnviarMensagemParaUsuario(
                atendimento.Canal,
                atendimento.IdUsuarioExterno,
                mensagem.Conteudo
            );

            return true;
        }

        public async Task<bool> TransferirAtendimentoAsync(int id, int paraAtendenteId)
        {
            var atendimento = await _uow.AtendimentoRepository.GetAsync(a => a.Id == id);

            if (atendimento == null)
                return false;

            var atendenteDestino = await _uow.AtendenteRepository.GetAsync(a => a.Id == paraAtendenteId);
            if (atendenteDestino == null)
                return false;

            atendimento.AtendenteId = atendenteDestino.Id;
            await _uow.AtendimentoRepository.UpdateAsync(atendimento);
            await _uow.CommitAsync();
            return true;

        }
        public async Task<bool> AssumirAtendimentoAsync(int id, Atendente atendente)
        {
            var atendimento = await _uow.AtendimentoRepository.GetAsync(a => a.Id == id);

            if (atendimento == null)
                return false;

            atendimento.AtendenteId = atendente.Id;
            await _uow.AtendimentoRepository.UpdateAsync(atendimento);
            await _uow.CommitAsync();
            return true;

        }

        public async Task<bool> RemoverAsync(int id)
        {
            var atendimento = await _uow.AtendimentoRepository.GetAsync(a => a.Id == id);

            if (atendimento == null)
                return false;

            await _uow.AtendimentoRepository.DeleteAsync(atendimento);
            await _uow.CommitAsync();
            return true;
        }
        public async Task<List<Atendimento>> ListarDoCliente(int clienteId)
        {
            return await _uow.AtendimentoRepository
                .GetQueryable(a => a.ClienteId == clienteId)
                .Include(a => a.Atendente)
                .ToListAsync();
        }

        public async Task<List<Atendimento>> ListarDoAtendente(int atendenteId)
        {
            return await _uow.AtendimentoRepository
                .GetQueryable(a => a.AtendenteId == atendenteId)
                .Include(a => a.Atendente)
                .ToListAsync();
        }

        public async Task<List<Atendimento>> FilaAtendimento()
        {
            var fila = await _uow.AtendimentoRepository
                .GetAllAsync(a => a.Status != AtendimentoStatus.Concluido);

            var filaOrdenada = fila.OrderBy(a => a.CriadoEm).ToList();

            return filaOrdenada;
        }

        public async Task<List<Mensagem>> ListarMensagensAsync(int atendimentoId)
        {
            var mensagens = await _uow.MensagemRepository
             .GetAllAsync(m => m.AtendimentoId == atendimentoId);

            var mensagensOrdenadas = mensagens.OrderBy(d => d.DataHora);
            return mensagensOrdenadas.ToList();

        }
    }
}
