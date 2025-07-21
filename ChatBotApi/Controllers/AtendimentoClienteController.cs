using System.Security.Claims;
using AutoMapper;
using ChatBotApi.DTOs;
using ChatBotApi.Models;
using ChatBotApi.Services.Implementations;
using ChatBotApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cliente, Admin")]
    [ApiVersion("1.0")]
    public class AtendimentoClienteController : ControllerBase
    {
        private readonly IAtendimentoService _atendimentoService;
        private readonly IDistribuidorService _distribuidorService;
        private readonly IClienteService _clienteService;
        private readonly IOcrService _ocrService;
        private readonly IMapper _mapper;
        private readonly IHubContext<chatHub> _hubContext;

        public AtendimentoClienteController(
            IAtendimentoService atendimentoService,
            IMapper mapper,
            IDistribuidorService distribuidorService,
            IClienteService clienteService,
            IOcrService ocrService,
            IHubContext<chatHub> hubContext)
        {
            _atendimentoService = atendimentoService;
            _mapper = mapper;
            _distribuidorService = distribuidorService;
            _clienteService = clienteService;
            _ocrService = ocrService;
            _hubContext = hubContext;
        }

        [HttpGet("meus")]
        public async Task<ActionResult<IEnumerable<AtendimentoDto>>> GetMeusAtendimentos([FromQuery] int? status = null)
        {
            var userModelId = ObterClienteIdLogado();
            var cliente = await _clienteService.ObterPorUserModelIdAsync(userModelId);

            var atendimentos = await _atendimentoService.ListarDoCliente(cliente.Id);

            if (status.HasValue)
            {
                atendimentos = atendimentos
                    .Where(a => (int)a.Status == status.Value)
                    .ToList();
            }

            var fila = await _atendimentoService.FilaAtendimento();
            var dtos = _mapper.Map<List<AtendimentoDto>>(atendimentos);

            foreach (var dto in dtos)
            {
                dto.PosicaoNaFila = fila.FindIndex(f => f.Id == dto.Id) + 1;
            }

            return Ok(dtos);
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<AtendimentoDto>> GetByFilter(string conteudo) 
        {
            var userModelId = ObterClienteIdLogado();
            var cliente = await _clienteService.ObterPorUserModelIdAsync(userModelId);

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
            try
            {
                var userModelId = ObterClienteIdLogado();
                var cliente = await _clienteService.ObterPorUserModelIdAsync(userModelId);

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
                    EnviadaPorAtendente = false
                };

                await _atendimentoService.ResponderClienteAsync(mensagem);

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

            var userModelId = ObterClienteIdLogado();
            var cliente = await _clienteService.ObterPorUserModelIdAsync(userModelId);

            if (cliente == null)
                return Unauthorized("Cliente não encontrado.");

            var atendimento = await _atendimentoService.ObterPorIdAsync(id);

            if (atendimento == null || atendimento.ClienteId != cliente.Id)
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
                EnviadoPor = cliente.Nome,
                DataHora = DateTime.Now,
                ClienteId = cliente.Id,
                EnviadaPorAtendente = false,
                ImagemUrl = imagemUrl
            };

            await _atendimentoService.ResponderClienteAsync(mensagem);

            await MensagemTempoReal(mensagem, atendimento);

            return Ok(new
            {
                Mensagem = "Enviado anexo com sucesso.",
                UrlImagem = imagemUrl
            });
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
