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
        private readonly IAtendimentoRepository _atendimentoRepo;
        private readonly IAtendenteRepository _atendenteRepo;
        private readonly IRepository<Mensagem> _mensagemRepo;
        private readonly IDistribuidorService _distribuidorService;
        private readonly ICanalService _canalService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;

        public AtendimentoService(
            IAtendimentoRepository atendimentoRepo,
            IAtendenteRepository atendenteRepo,
            IRepository<Mensagem> mensagemRepo,
            IDistribuidorService distribuidorService,
            ICanalService canalService,
            IMapper mapper,
            IUnitOfWork uof)
        {
            _atendimentoRepo = atendimentoRepo;
            _atendenteRepo = atendenteRepo;
            _mensagemRepo = mensagemRepo;
            _distribuidorService = distribuidorService;
            _canalService = canalService;
            _mapper = mapper;
            _uof = uof;
        }

        public async Task CriarOuEncaminharAtendimentoAsync(MensagemEntradaDto dto)
        {
            var atendimento = await _atendimentoRepo.BuscarAtivoPorUsuarioExterno(dto.IdUsuarioExterno);

            if (atendimento == null)
            {
                var atendente = await _distribuidorService.ObterAtendenteDisponivelAsync();
                if (atendente == null)
                {
                    throw new Exception("Nenhum atendente disponível.");
                }

                atendimento = new Atendimento
                {
                    Canal = dto.Canal,
                    IdUsuarioExterno = dto.IdUsuarioExterno,
                    NomeUsuario = dto.NomeUsuario,
                    AtendenteId = atendente.Id,
                    Status = AtendimentoStatus.Iniciado,
                    CriadoEm = DateTime.Now,
                    Mensagens = new List<Mensagem>()
                };
                _atendimentoRepo.Create(atendimento); // Removed 'await' as Create is not asynchronous
            }
            var mensagem = _mapper.Map<Mensagem>(dto);
            mensagem.AtendimentoId = atendimento.Id;
            mensagem.EnviadaPorAtendente = false;

            _mensagemRepo.Create(mensagem); // Removed 'await' as Create is not asynchronous
            await _uof.CommitAsync();
        }


        public async Task EnviarRespostaDoAtendenteAsync(RespostaAtendenteDto dto)
        {
            var atendimento = await _atendimentoRepo.GetAsync(a => a.Id == dto.AtendimentoId);
            if (atendimento == null)
                throw new Exception("Atendimento não encontrado.");

            var mensagem = new Mensagem
            {
                AtendimentoId = dto.AtendimentoId,
                Conteudo = dto.Mensagem,
                DataHora = DateTime.Now,
                EnviadaPorAtendente = true
            };

            await _mensagemRepo.AddAsync(mensagem);
            await _uof.CommitAsync();

            await _canalService.EnviarMensagemParaUsuario(atendimento.Canal, atendimento.IdUsuarioExterno, dto.Mensagem);
        }
    }
    }
}
