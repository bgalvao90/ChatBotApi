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
    public class AtendenteController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _uof;

        public AtendenteController(AppDbContext context, IUnitOfWork uof)
        {
            _context = context;
            _uof = uof;
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
            await _uof.CommitAsync();

            return Ok(new
            {
                AtendenteId = atendente.Id,
                UsuarioId = usuario.Id
            });
        }
    }

}
