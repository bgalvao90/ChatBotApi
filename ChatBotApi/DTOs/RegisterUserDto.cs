using System.ComponentModel.DataAnnotations;
using ChatBotApi.Models.Enums;

namespace ChatBotApi.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public RoleUsuario Role { get; set; }
    }
}
