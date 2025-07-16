using ChatBotApi.Context;
using ChatBotApi.DTOs;
using ChatBotApi.Models;
using ChatBotApi.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("registrar")]
        public async Task<ActionResult> RegistrarCliente([FromBody] RegisterClienteDto dto)
        {
            if (_context.Usuarios.Any(u => u.UserName == dto.UserName))
                return Conflict("Usuário já existe.");

            var usuario = new UserModel
            {
                UserName = dto.UserName,
                Password = dto.Password,
                Role = RoleUsuario.Cliente
            };

            var cliente = new Cliente
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Usuario = usuario
            };

            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ClienteId = cliente.Id,
                UsuarioId = usuario.Id
            });
        }
    }
}
