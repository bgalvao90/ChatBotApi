using ChatBotApi.Models.Enums;

namespace ChatBotApi.Models
{
    public class Atendimento
    {
        public int Id { get; set; }
        public string? Canal { get; set; }
        public string? IdUsuarioExterno { get; set; }
        public string NomeUsuario { get; set; }
        public int AtendenteId { get; set; }
        public Atendente Atendente { get; set; }
        public string Titulo { get; set; }
        public string Categoria { get; set; }
        public string Observacao { get; set; }
        public AtendimentoStatus  Status { get; set; }
        public DateTime CriadoEm { get; set; }
        public int ClienteId { get; set; } 
        public Cliente Cliente { get; set; }
        public List<Mensagem> Mensagens { get; set; } = new List<Mensagem>();

    }
}
