using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChatBotApi.Services.Interfaces;

namespace ChatBotApi.Services.Implementations
{
    public class CanalService : ICanalService
    {
        private readonly HttpClient _httpClient;

        public CanalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task EnviarMensagemParaUsuario(string canal, string idExterno, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(canal) || canal.ToLower() != "site")
                return;
            var payload = new
            {
                usuarioId = idExterno,
                conteudo = mensagem,
            };

            // Endpoint onde seu front ou outro serviço receberá a mensagem
            var url = "https://seudominio.com/api/site/notificar"; // <-- Altere conforme sua infra

            await _httpClient.PostAsJsonAsync(url, payload);
        }
    }
}
