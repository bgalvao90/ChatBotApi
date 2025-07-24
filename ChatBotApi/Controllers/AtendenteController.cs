using ChatBotApi.Context;
using ChatBotApi.DTOs;
using ChatBotApi.Models;
using ChatBotApi.Models.Enums;
using ChatBotApi.Repositories.Interfaces;
using ChatBotApi.Services.Implementations;
using ChatBotApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AtendenteController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _uow;
        private readonly IAtendenteService _atendenteService;

        public AtendenteController(AppDbContext context, IUnitOfWork uow, IAtendenteService atendenteService)
        {
            _context = context;
            _uow = uow;
            _atendenteService = atendenteService;
        }

        [HttpPost("registrar")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> RegistrarAtendente([FromBody] RegisterAtendenteDto dto)
        {
            if (_context.Usuarios.Any(u => u.UserName == dto.UserName))
                return Conflict("Usuário já existe.");

            var usuario = new UserModel
            {
                UserName = dto.UserName,
                Password = dto.Password,
                Role = RoleUsuario.Atendente
            };

            var atendente = new Atendente
            {
                Nome = dto.Nome,
                Usuario = usuario,
                Status = AtendenteStatus.Online,
                Disponivel = true,
                Funcao = dto.Funcao
            };

            await _context.Atendentes.AddAsync(atendente);
            await _uow.CommitAsync();

            return Ok(new
            {
                AtendenteId = atendente.Id,
                UsuarioId = usuario.Id
            });
        }

        [HttpGet("atendentes")]
        public async Task<ActionResult<Atendente>> GetAllAsync()
        {
            var atendentes = await _uow.AtendenteRepository.GetAllAsync();

            if (atendentes is null)
                return NotFound("Não existe atendentes cadastrados.");

            return Ok(atendentes.ToList());
        }

        [HttpPatch("status")]
        public async Task<ActionResult<Atendente>> AlterarStatus(int id, [FromQuery] AtendenteStatus status)
        {
            try
            {
                var userModelId = ObterAtendenteIdLogado();
                var atendente = await _atendenteService.ObterPorUserModelIdAsync(userModelId);

                var alterado = await _atendenteService.AtualizarStatusAsync(atendente.Id, status);

                if (!alterado)
                    return NotFound(new { Erro = "Atendente não encontrado." });


                return Ok(new
                {
                    Mensagem = "Status alterado com sucesso.",
                    NovoStatus = status.ToString()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = ex.Message });
            }
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
