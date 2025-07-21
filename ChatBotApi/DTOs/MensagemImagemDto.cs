namespace ChatBotApi.DTOs
{
    public class MensagemImagemDto
    {
        public string? Conteudo { get; set; }
        public IFormFile? Imagem { get; set; }
    }

}
