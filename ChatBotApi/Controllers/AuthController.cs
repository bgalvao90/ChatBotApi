using System.Linq;
using ChatBotApi.Context;
using ChatBotApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(ITokenService tokenService, IConfiguration configuration, AppDbContext context)
        {
            _tokenService = tokenService;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody]LoginDto user)
        {
            if (user is null)
            {
                return BadRequest("Login inválido");
            }

            var usuarioEncontrado = _context.Usuarios.FirstOrDefault(u => u.UserName == user.UserName);

            if (usuarioEncontrado == null)
            {
                return Unauthorized("Usuário não encontrado.");
            }
            if (usuarioEncontrado.Password != user.Password)
            {
                return Unauthorized("Senha incorreta.");
            }

                var tokenString = _tokenService.GerarToken(
                    _configuration["Jwt:Key"],
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    usuarioEncontrado);

                return Ok(new { Token = tokenString,
                Nome = usuarioEncontrado.UserName,
                Role = usuarioEncontrado.Role.ToString()});
        }
        //[Authorize(Roles = "Admin")]
        [HttpPost("CriarUsuario")]
        public async Task<ActionResult> CreateUser([FromBody] RegisterUserDto user)
        {
            if (user == null)
            {
                return BadRequest("Usuário inválido.");
            }

            var usuarioExistente = _context.Usuarios.FirstOrDefault(u => u.UserName == user.UserName);

            if (usuarioExistente != null)
            {
                return Conflict("Usuário já existe.");
            }

            var novoUsuario = new UserModel
            {
                UserName = user.UserName,
                Password = user.Password,
                Role = user.Role
            };

            await _context.Usuarios.AddAsync(novoUsuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateUser), new { id = novoUsuario.Id }, novoUsuario);
        }
    }
}
