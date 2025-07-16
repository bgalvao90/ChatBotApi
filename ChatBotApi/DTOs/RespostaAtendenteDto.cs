namespace ChatBotApi.DTOs
{
    public class RespostaAtendenteDto
    {
        public int AtendimentoId { get; set; }
        public string Conteudo { get; set; }
        public string EnviadoPor { get; set; }
        public DateTime DataHora { get; set; } = DateTime.UtcNow;

    }
}
