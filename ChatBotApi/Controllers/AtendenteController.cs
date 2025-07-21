using ChatBotApi.Context;
using ChatBotApi.DTOs;
using ChatBotApi.Models;
using ChatBotApi.Models.Enums;
using ChatBotApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AtendenteController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _uow;

        public AtendenteController(AppDbContext context, IUnitOfWork uow)
        {
            _context = context;
            _uow = uow;
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
                Funcao = "Suporte"
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
    }

}
