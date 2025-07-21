using System.Security.Claims;
using AutoMapper;
using ChatBotApi.Context;
using ChatBotApi.DTOs;
using ChatBotApi.Models;
using ChatBotApi.Models.Enums;
using ChatBotApi.Repositories.Interfaces;
using ChatBotApi.Services.Implementations;
using ChatBotApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize (Roles = "Atendente, Admin")]
    [ApiVersion("1.0")]

    public class AtendimentoAtendenteController : ControllerBase
    {
        private readonly IAtendimentoService _atendimentoService;
        private readonly IDistribuidorService _distribuidorService;
        private readonly IAtendenteService _atendenteService;
        private readonly IMapper _mapper;
        private readonly IHubContext<chatHub> _hubContext;

        public AtendimentoAtendenteController(IAtendimentoService atendimentoService, IMapper mapper, IDistribuidorService distribuidorService, IAtendenteService atendenteService = null, IHubContext<chatHub> hubContext = null)
        {
            _atendimentoService = atendimentoService;
            _mapper = mapper;
            _distribuidorService = distribuidorService;
            _atendenteService = atendenteService;
            _hubContext = hubContext;
        }


        [HttpGet("meus")]
        public async Task<ActionResult<IEnumerable<AtendimentoDto>>> ObterMeusAtendimentos([FromQuery] List<int> status = null)
        {
            var userModelId = ObterAtendenteIdLogado();
            var atendente = await _atendenteService.ObterPorUserModelIdAsync(userModelId);

            var meusAtendimentos = await _atendimentoService.ListarDoAtendente(atendente.Id);

            if (status != null && status.Any())
            {
                meusAtendimentos = meusAtendimentos
                    .Where(a => status.Contains((int)a.Status))
                    .ToList();
            }

            var fila = await _atendimentoService.FilaAtendimento();
            var atendimentoDtos = _mapper.Map<List<AtendimentoDto>>(meusAtendimentos);

            foreach (var dto in atendimentoDtos)
            {
                dto.PosicaoNaFila = fila.FindIndex(f => f.Id == dto.Id) + 1;
            }

            return Ok(atendimentoDtos);
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<AtendimentoDto>> GetByFilter(string conteudo)
        { 
            var atendimentos = await _atendimentoService.ListaMensagemFiltro(conteudo);
            if (atendimentos == null)
            {
                Console.WriteLine("Atendimento não encontrado.");
                return NotFound();
            }

            var atendimentosDto = _mapper.Map<List<AtendimentoDto>>(atendimentos);

            return Ok(atendimentosDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AtendimentoDto>> GetById(int id)
        {
            var atendimento = await _atendimentoService.ObterPorIdAsync(id);
            if (atendimento == null)
            {
                return NotFound();
            }

            var atendimentoDto = _mapper.Map<AtendimentoDto>(atendimento);
            return Ok(atendimentoDto);
        }

        [HttpGet("{id}/mensagens")]
        public async Task<ActionResult<IEnumerable<MensagemDto>>> ObterMensagens(int id)
        {
            try
            {
                var userModelId = ObterAtendenteIdLogado();
                var atendente = await _atendenteService.ObterPorUserModelIdAsync(userModelId);

                var atendimento = await _atendimentoService.ObterPorIdAsync(id);
                if (atendimento == null || atendimento.AtendenteId != atendente.Id)
                    return NotFound();

                var mensagens = await _atendimentoService.ListarMensagensAsync(atendimento.Id);
                var mensagensDto = _mapper.Map<List<MensagemDto>>(mensagens);


                return Ok(mensagensDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = ex.Message });
            }
        }


        [HttpPost("criar")]
        public async Task<ActionResult<AtendimentoDto>> CriarAtendimentoAsync([FromBody] MensagemEntradaDto entrada)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userModelId = ObterAtendenteIdLogado();

                var atendente = await _atendenteService.ObterPorUserModelIdAsync(userModelId);

                var mensagem = new Mensagem
                {
                    Canal = "site",
                    IdUsuarioExterno = entrada.IdUsuarioExterno,
                    Conteudo = entrada.Conteudo,
                    EnviadoPor = atendente.Nome,
                    DataHora = entrada.DataHora,
                    ClienteId = atendente.Id,
                    EnviadaPorAtendente = true
                };

                var atendimento = await _distribuidorService.CriarAtendimentoAsync(mensagem);
                var fila = await _atendimentoService.FilaAtendimento();
                var posicaoFila = fila.FindIndex(a => a.Id == atendimento.Id) + 1;

                if (atendimento == null)
                    return StatusCode(500, new { Erro = "Erro ao criar atendimento." });

                var atendimentoDto = _mapper.Map<AtendimentoDto>(atendimento);
                atendimentoDto.PosicaoNaFila = posicaoFila;
                await MensagemTempoReal(mensagem, atendimento);

                return CreatedAtAction(nameof(GetById), new { id = atendimento.Id }, atendimentoDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = ex.Message });
            }
        }

        private async Task MensagemTempoReal(Mensagem mensagem, Atendimento atendimento)
        {
            var mensagemDto = _mapper.Map<MensagemDto>(mensagem);

            await _hubContext.Clients.Group(atendimento.Id.ToString())
                    .SendAsync("NovaMensagem", mensagemDto);
        }

        [HttpPatch("alterar-status/{id}")]
        public async Task<ActionResult> AlterarStatusAtendimento(int id, [FromQuery] AtendimentoStatus statusAtendimento)
        {
            try
            {
                var alterado = await _atendimentoService.StatusAtendimentoAsync(id, statusAtendimento);

                if (!alterado)
                    return NotFound(new { Erro = "Atendimento não encontrado." });

                return Ok(new { Mensagem = "Status alterado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = ex.Message });
            }
        }

        [HttpGet("pendentes")]
        public async Task<ActionResult<IEnumerable<AtendimentoDto>>> ListarPendentes()
        {

            var atendimentos = await _atendimentoService.ListarPendentesAsync();
            var fila = await _atendimentoService.FilaAtendimento();

            var atendimentoDtos = _mapper.Map<List<AtendimentoDto>>(atendimentos);
            foreach (var dto in atendimentoDtos)
            {
                var index = fila.FindIndex(f => f.Id == dto.Id);
                if (index >= 0)
                {
                    dto.PosicaoNaFila = index + 1;
                }
                else
                {
                    dto.PosicaoNaFila = 0;
                }
            }
            return Ok(atendimentoDtos);
        }

        [HttpPost("{id}/responder")]
        public async Task<ActionResult> ResponderAtendimento(int id, [FromBody] MensagemEntradaDto dto)
        {
            try { 
            var userModelId = ObterAtendenteIdLogado();

            var atendente = await _atendenteService.ObterPorUserModelIdAsync(userModelId);

            var atendimento = await _atendimentoService.ObterPorIdAsync(id);
            if (atendimento == null || atendimento.AtendenteId != atendente.Id)
                return NotFound();

            var mensagem = new Mensagem
            {
                Canal = "site",
                IdUsuarioExterno = dto.IdUsuarioExterno,
                Conteudo = dto.Conteudo,
                EnviadoPor = atendente.Nome,
                DataHora = dto.DataHora,
                AtendimentoId = atendimento.Id,
                EnviadaPorAtendente = true
            };
            await _atendimentoService.ResponderAtendenteAsync(mensagem);
                await MensagemTempoReal(mensagem, atendimento);

                return Ok(new { Mensagem = "Mensagem enviada com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = ex.Message });
            }
        }

        [HttpPost("{id}/anexo")]
        public async Task<IActionResult> Anexo(int id, [FromForm] MensagemImagemDto dto)
        {
            if (dto == null)
                return BadRequest("Dados inválidos.");

            var userModelId = ObterAtendenteIdLogado();
            var atendente = await _atendenteService.ObterPorUserModelIdAsync(userModelId);

            if (atendente == null)
                return Unauthorized("Cliente não encontrado.");

            var atendimento = await _atendimentoService.ObterPorIdAsync(id);

            if (atendimento == null || atendimento.AtendenteId != atendente.Id)
                return NotFound("Atendimento não encontrado ou não pertence ao cliente.");

            string imagemUrl = null;

            if (dto.Imagem != null && dto.Imagem.Length > 0)
            {
                try
                {
                    var nomeArquivo = $"{Guid.NewGuid()}.jpg";
                    var pastaImagens = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagens");

                    if (!Directory.Exists(pastaImagens))
                        Directory.CreateDirectory(pastaImagens);

                    var caminhoCompleto = Path.Combine(pastaImagens, nomeArquivo);

                    using var stream = new FileStream(caminhoCompleto, FileMode.Create);
                    await dto.Imagem.CopyToAsync(stream);

                    imagemUrl = $"/imagens/{nomeArquivo}";


                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Erro ao processar a imagem: {ex.Message}");
                }
            }

            var mensagem = new Mensagem
            {
                AtendimentoId = id,
                Canal = "site",
                IdUsuarioExterno = atendimento.IdUsuarioExterno,
                Conteudo = "Anexo",
                EnviadoPor = atendente.Nome,
                DataHora = DateTime.Now,
                EnviadaPorAtendente = true,
                ImagemUrl = imagemUrl
            };

            await _atendimentoService.ResponderAtendenteAsync(mensagem);

            await MensagemTempoReal(mensagem, atendimento);

            return Ok(new
            {
                Mensagem = "Enviado anexo com sucesso.",
                UrlImagem = imagemUrl
            });
        }

        [HttpPatch("Finalizar-Atendimento/{id}")]
        public async Task<ActionResult> FinalizarAtendimento(int id)
        {
            try
            {
                var alterado = await _atendimentoService.FinalizarAtendimentoAsync(id);

                if (!alterado)
                    return NotFound(new { Erro = "Atendimento não encontrado." });

                
                return Ok(new { Mensagem = "Atendimento finalizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = ex.Message });
            }
        }

        [HttpPatch("transferir/{id}")]
        public async Task<ActionResult> TransferirAtendimento(int id, [FromQuery] int paraAtendenteId)
        {
            try
            {
                var sucesso = await _atendimentoService.TransferirAtendimentoAsync(id, paraAtendenteId);
                if (!sucesso)
                    return NotFound(new { Erro = "Atendimento não encontrado ou atendente inválido." });

                return Ok(new { Mensagem = "Atendimento transferido com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = ex.Message });
            }
        }


        [HttpPatch("assumir/{id}")]
        public async Task<ActionResult> AssumirAtendimento(int id)
        {
            var userModelId = ObterAtendenteIdLogado();
            var atendente = await _atendenteService.ObterPorUserModelIdAsync(userModelId);

            var sucesso = await _atendimentoService.AssumirAtendimentoAsync(id, atendente);

            if (!sucesso)
                return BadRequest(new { Erro = "Não foi possível assumir o atendimento." });

            return Ok(new { Mensagem = "Atendimento assumido com sucesso." });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoverAtendimento(int id)
        {
            var sucesso = await _atendimentoService.RemoverAsync(id);
            if (!sucesso)
                return NotFound(new { Erro = "Atendimento não encontrado." });

            return NoContent();
        }

        private int ObterAtendenteIdLogado()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int AtendenteId))
                throw new UnauthorizedAccessException("Usuário não autorizado ou ID inválido.");

            return AtendenteId;
        }
    }
}




