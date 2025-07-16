using AutoMapper;
using ChatBotApi.Context;
using ChatBotApi.DTOs;
using ChatBotApi.Models;
using ChatBotApi.Models.Enums;
using ChatBotApi.Repositories.Interfaces;
using ChatBotApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AtendimentoController : ControllerBase
    {
        private readonly IAtendimentoService _atendimentoService;
        private readonly IDistribuidorService _distribuidorService;
        private readonly IMapper _mapper;

        public AtendimentoController(IAtendimentoService atendimentoService, IMapper mapper, IDistribuidorService distribuidorService)
        {
            _atendimentoService = atendimentoService;
            _mapper = mapper;
            _distribuidorService = distribuidorService;
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
                var mensagem = new Mensagem
                {
                    Canal = "site",
                    IdUsuarioExterno = entrada.IdUsuarioExterno,
                    Conteudo = entrada.Conteudo,
                    EnviadoPor = entrada.EnviadoPor,
                    DataHora = entrada.DataHora,
                    EnviadaPorAtendente = false // ou defina conforme seu contexto
                };

                var atendimento = await _distribuidorService.CriarAtendimentoAsync(mensagem);

                if (atendimento == null)
                    return StatusCode(500, new { Erro = "Erro ao criar atendimento." });

                var atendimentoDto = _mapper.Map<AtendimentoDto>(atendimento);

                return CreatedAtAction(nameof(GetById), new { id = atendimento.Id }, atendimentoDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = ex.Message });
            }
        }
        [HttpPost("responder")]
        public async Task<ActionResult> ResponderClienteAsync([FromBody] RespostaAtendenteDto respostaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var sucesso = await _atendimentoService.ResponderClienteAsync(respostaDto);
                if (!sucesso)
                    return NotFound(new { Erro = "Atendimento não encontrado." });

                return Ok(new { Mensagem = "Resposta enviada com sucesso." });
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
    }
}



