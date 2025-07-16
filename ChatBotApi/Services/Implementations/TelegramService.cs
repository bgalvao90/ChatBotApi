using ChatBotApi.Services.Interfaces;

namespace ChatBotApi.Services.Implementations
{
    public class TelegramService : ICanalService
    {
        public async Task EnviarMensagemParaUsuario(string canal, string idExterno, string mensagem)
        {
            // Aqui você faz chamada à API do Telegram usando HttpClient
            if (canal.ToLower() != "telegram") return;

            var url = $"https://api.telegram.org/bot7627120584:AAGuBKR1sherIxFIT9G2HXpoNFfO3fvhRN4/sendMessage";
            var payload = new
            {

                chat_id = idExterno,
                text = mensagem
            };

            using var client = new HttpClient();
            await client.PostAsJsonAsync(url, payload);
        }

    }
}