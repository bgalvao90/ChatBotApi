using ChatBotApi.Models.Enums;

namespace ChatBotApi.Models
{
    public class Atendimento
    {
        public int Id { get; set; }
        public string Canal { get; set; }
        public string IdUsuarioExterno { get; set; }
        public string NomeUsuario { get; set; }
        public int AtendenteId { get; set; }
        public AtendimentoStatus  Status { get; set; }
        public List<Mensagem> Mensagens { get; set; }

    }
}
