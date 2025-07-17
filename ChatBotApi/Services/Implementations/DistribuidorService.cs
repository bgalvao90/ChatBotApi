using ChatBotApi.Models;
using ChatBotApi.Models.Enums;
using ChatBotApi.Repositories.Interfaces;
using ChatBotApi.Services.Interfaces;

namespace ChatBotApi.Services.Implementations
{
    public class DistribuidorService : IDistribuidorService
    {
        private readonly IIAService _iaService;
        private readonly IUnitOfWork _uof;

        public DistribuidorService(IIAService iaService, IUnitOfWork uof)
        {
            _iaService = iaService;
            _uof = uof;
        }

        public async Task<Atendimento?> CriarAtendimentoAsync(Mensagem mensagem)
        {
            var (categoria, titulo, resumo) = await _iaService.ClassificarMensagemAsync(mensagem.Conteudo);

            var atendentesDisponiveis = await _uof.AtendenteRepository.ObterAtendenteComMenorAtendimentosAsync();

            if (atendentesDisponiveis == null)
            {
                throw new Exception("Nenhum atendete disponível.");
            }

            var novoAtendimento = new Atendimento
            {
                Canal = string.IsNullOrEmpty(mensagem.Canal) ? "Desconhecido" : mensagem.Canal,
                IdUsuarioExterno = string.IsNullOrEmpty(mensagem.IdUsuarioExterno) ? "Não Informado" : mensagem.IdUsuarioExterno,
                Titulo = titulo,
                Categoria = categoria,
                Observacao = resumo,
                NomeUsuario = mensagem.EnviadoPor,
                CriadoEm = DateTime.Now,
                Status = AtendimentoStatus.Iniciado,
                AtendenteId = atendentesDisponiveis.Id,
                ClienteId = mensagem.ClienteId,
                Mensagens = new List<Mensagem> { mensagem },
            };

            await _uof.AtendimentoRepository.CreateAsync(novoAtendimento);
            await _uof.CommitAsync();

            return novoAtendimento;
        }
    }
}
