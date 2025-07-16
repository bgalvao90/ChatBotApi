namespace ChatBotApi.DTOs
{
    public class MensagemDto
    {
        public int Id { get; set; }
        public string Conteudo { get; set; }
        public string EnviadoPor { get; set; }
        public bool EnviadaPorAtendente { get; set; }
        public DateTime DataHora { get; set; }
    }
}
