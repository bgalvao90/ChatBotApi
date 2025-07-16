using ChatBotApi.Models.Enums;

namespace ChatBotApi.DTOs
{
    public class AtendimentoDto
    {
        public int Id { get; set; }
        public string Canal { get; set; }
        public string IdUsuarioExterno { get; set; }
        public string NomeUsuario { get; set; }
        public int AtendenteId { get; set; }
        public string Titulo { get; set; }
        public string Categoria { get; set; }
        public string Observacao { get; set; }
        public AtendimentoStatus Status { get; set; } = AtendimentoStatus.Iniciado;
        public DateTime CriadoEm { get; set; }
        public List<MensagemDto> Mensagens { get; set; } = new List<MensagemDto>();
    }
}
