using System.Security.Claims;
using AutoMapper;
using ChatBotApi.DTOs;
using ChatBotApi.Models;
using ChatBotApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cliente, Admin")]
    public class AtendimentoClienteController : ControllerBase
    {
        private readonly IAtendimentoService _atendimentoService;
        private readonly IDistribuidorService _distribuidorService;
        private readonly IClienteService _clienteService;
        private readonly IMapper _mapper;

        public AtendimentoClienteController(
            IAtendimentoService atendimentoService,
            IMapper mapper,
            IDistribuidorService distribuidorService,
            IClienteService clienteService)
        {
            _atendimentoService = atendimentoService;
            _mapper = mapper;
            _distribuidorService = distribuidorService;
            _clienteService = clienteService;
        }

        [HttpGet("meus")]
        public async Task<ActionResult<IEnumerable<AtendimentoDto>>> GetMeusAtendimentos()
        {
            var userModelId = ObterClienteIdLogado();
            var cliente = await _clienteService.ObterPorUserModelIdAsync(userModelId);

            var atendimentos = await _atendimentoService.ListarDoCliente(cliente.Id);
            var fila = await _atendimentoService.FilaAtendimento();

            var dtos = _mapper.Map<List<AtendimentoDto>>(atendimentos);

            foreach (var dto in dtos)
            {
                dto.PosicaoNaFila = fila.FindIndex(f => f.Id == dto.Id) + 1;
            }

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AtendimentoDto>> GetById(int id)
        {
            try
            {
                var userModelId = ObterClienteIdLogado();
                var cliente = await _clienteService.ObterPorUserModelIdAsync(userModelId);
                Console.WriteLine($"Cliente ID do token: {cliente}");

                var atendimento = await _atendimentoService.ObterPorIdAsync(id);


                if (atendimento == null)
                {
                    Console.WriteLine("Atendimento não encontrado.");
                    return NotFound();
                }

                if (atendimento.ClienteId != cliente.Id)
                {
                    Console.WriteLine($"Atendimento pertence a outro cliente: {atendimento.ClienteId}");
                    return NotFound();
                }

                var fila = await _atendimentoService.FilaAtendimento();
                var posicaoFila = fila.FindIndex(a => a.Id == atendimento.Id) + 1;

                var atendimentoDto = _mapper.Map<AtendimentoDto>(atendimento);
                atendimentoDto.PosicaoNaFila = posicaoFila;

                return Ok(atendimentoDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
                return StatusCode(500, new { erro = ex.Message });
            }
        }


        [HttpPost("criar")]
        public async Task<ActionResult<AtendimentoDto>> CriarAtendimentoAsync([FromBody] MensagemEntradaDto entrada)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userModelId = ObterClienteIdLogado();

                var cliente = await _clienteService.ObterPorUserModelIdAsync(userModelId);

                var mensagem = new Mensagem
                {
                    Canal = "site",
                    IdUsuarioExterno = entrada.IdUsuarioExterno,
                    Conteudo = entrada.Conteudo,
                    EnviadoPor = cliente.Nome,
                    DataHora = entrada.DataHora,
                    ClienteId = cliente.Id,
                    EnviadaPorAtendente = false
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


        [HttpGet("pendentes")]
        public async Task<ActionResult<IEnumerable<AtendimentoDto>>> ListarPendentes()
        {
            var userModelId = ObterClienteIdLogado();

            var cliente = await _clienteService.ObterPorUserModelIdAsync(userModelId);

            var atendimentos = await _atendimentoService.ListarPendentesClienteAsync(cliente.Id);
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
        public async Task<ActionResult> ResponderAtendimento(int id, [FromBody] MensagemEntradaDto entrada)
        {
            try
            {
                var userModelId = ObterClienteIdLogado();
                var cliente = await _clienteService.ObterPorUserModelIdAsync(userModelId);

                var atendimento = await _atendimentoService.ObterPorIdAsync(id);
                if (atendimento == null || atendimento.ClienteId != cliente.Id)
                    return NotFound();

                var mensagem = new Mensagem
                {
                    Canal = "site",
                    IdUsuarioExterno = entrada.IdUsuarioExterno,
                    Conteudo = entrada.Conteudo,
                    EnviadoPor = cliente.Nome,
                    DataHora = entrada.DataHora,
                    ClienteId = cliente.Id,
                    AtendimentoId = atendimento.Id,
                    EnviadaPorAtendente =  false
                };

                await _atendimentoService.ResponderClienteAsync(mensagem);

                return Ok(new { Mensagem = "Mensagem enviada com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = ex.Message });
            }
        }

        [HttpGet("{id}/mensagens")]
        public async Task<ActionResult<IEnumerable<MensagemDto>>> ObterMensagens(int id)
        {
            try
            {
                var userModelId = ObterClienteIdLogado();
                var cliente = await _clienteService.ObterPorUserModelIdAsync(userModelId);

                var atendimento = await _atendimentoService.ObterPorIdAsync(id);
                if (atendimento == null || atendimento.ClienteId != cliente.Id)
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


        private int ObterClienteIdLogado()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int clienteId))
                throw new UnauthorizedAccessException("Usuário não autorizado ou ID inválido.");

            return clienteId;
        }
    }
}
