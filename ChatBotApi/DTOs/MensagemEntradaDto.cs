namespace ChatBotApi.DTOs
{
    public class MensagemEntradaDto
    {
        public string? Canal { get; set; }   // telegram, discord, etc.
        public string? IdUsuarioExterno { get; set; }   // ID do usuário no canal
        public string? NomeUsuario { get; set; }        // Nome do usuário (se disponível)
        public string? Conteudo { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;

    }
}
