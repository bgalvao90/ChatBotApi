using ChatBotApi.Context;
using ChatBotApi.DTOs;
using ChatBotApi.Models;
using ChatBotApi.Models.Enums;
using ChatBotApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _uow;

        public ClienteController(AppDbContext context, IUnitOfWork uow = null)
        {
            _context = context;
            _uow = uow;
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

        [HttpGet("clientes")]
        public async Task<ActionResult<Atendente>> GetAllAsync()
        {
            var clientes = await _uow.ClienteRepository.GetAllAsync();

            if (clientes is null)
                return NotFound("Não existe clientes cadastrados.");

            return Ok(clientes.ToList());
        }
    }
}
