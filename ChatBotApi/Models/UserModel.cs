using System.ComponentModel.DataAnnotations;
using ChatBotApi.Models;
using ChatBotApi.Models.Enums;

namespace ChatBotApi
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool? IsAdmin => Role == RoleUsuario.Admin;
        [Required]
        public RoleUsuario Role { get; set; }
        public Atendente? Atendente { get; set; }
        public Cliente? Cliente { get; set; }

    }
}
