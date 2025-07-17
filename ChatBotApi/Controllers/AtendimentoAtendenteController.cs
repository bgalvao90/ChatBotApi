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
using Microsoft.EntityFrameworkCore;

namespace ChatBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize (Roles = "Atendente, Admin")]

    public class AtendimentoAtendenteController : ControllerBase
    {
        private readonly IAtendimentoService _atendimentoService;
        private readonly IDistribuidorService _distribuidorService;
        private readonly IAtendenteService _atendenteService;
        private readonly IMapper _mapper;

        public AtendimentoAtendenteController(IAtendimentoService atendimentoService, IMapper mapper, IDistribuidorService distribuidorService, IAtendenteService atendenteService = null)
        {
            _atendimentoService = atendimentoService;
            _mapper = mapper;
            _distribuidorService = distribuidorService;
            _atendenteService = atendenteService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AtendimentoDto>>> GetAllAsync()
        {
            var atendimentos = await _atendimentoService.ObterAtendimentosAsync();
            if (atendimentos == null || !atendimentos.Any())
            {
                return NotFound("Não existem atendimentos.");
            }

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

                return CreatedAtAction(nameof(GetById), new { id = atendimento.Id }, atendimentoDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = ex.Message });
            }
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
                EnviadoPor = dto.NomeUsuario,
                DataHora = dto.DataHora,
                AtendimentoId = atendimento.Id,
                EnviadaPorAtendente = true
            };
            await _atendimentoService.ResponderAtendenteAsync(mensagem);

            return Ok(new { Mensagem = "Mensagem enviada com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = ex.Message });
            }
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

        [HttpGet("meus")]
        public async Task<ActionResult<IEnumerable<AtendimentoDto>>> ObterMeusAtendimentos()
        {
            var userModelId = ObterAtendenteIdLogado();
            var atendente = await _atendenteService.ObterPorUserModelIdAsync(userModelId);

            var meusAtendimentos = (await _atendimentoService.ListarDoAtendente(atendente.Id))
                .Where(a => a.Status != AtendimentoStatus.Concluido)
                .ToList();


            var fila = await _atendimentoService.FilaAtendimento();
            var atendimentoDtos = _mapper.Map<List<AtendimentoDto>>(meusAtendimentos);

            foreach (var dto in atendimentoDtos)
            {
                dto.PosicaoNaFila = fila.FindIndex(f => f.Id == dto.Id) + 1;
            }

            return Ok(atendimentoDtos);
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




