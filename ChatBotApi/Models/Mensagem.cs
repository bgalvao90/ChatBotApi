using System.Text.Json.Serialization;

namespace ChatBotApi.Models
{
    public class Mensagem
    {
        public int Id { get; set; }
        public int AtendimentoId { get; set; }
        public string? Canal { get; set; }        
        public string? IdUsuarioExterno { get; set; }
        public string Conteudo { get; set; }
        public DateTime DataHora { get; set; }
        public string EnviadoPor { get; set; }
        public int ClienteId { get; set; }
        public bool EnviadaPorAtendente { get; set; }
        [JsonIgnore]
        public Atendimento Atendimento { get; set; }
    }
}
