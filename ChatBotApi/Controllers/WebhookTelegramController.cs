using ChatBotApi.Models;
using ChatBotApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace ChatBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class WebhookTelegramController : ControllerBase
    {
        private readonly IDistribuidorService _distribuidorService;

        public WebhookTelegramController(IDistribuidorService distribuidorService)
        {
            _distribuidorService = distribuidorService;
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("API está viva!");

        [HttpPost]
        public IActionResult Post([FromBody] Update update)
        {
            Console.WriteLine("Recebido update do Telegram");
            if (update == null || update.Message == null || string.IsNullOrEmpty(update.Message.Text))
            {
                Console.WriteLine("Update inválido");
                return Ok();
            }
            Console.WriteLine($"Mensagem recebida: {update.Message.Text}");
            var mensagem = new Mensagem
            {
                Canal = "telegram",
                IdUsuarioExterno = update.Message.Chat.Id.ToString(),
                Conteudo = update.Message.Text,
                EnviadoPor = update.Message.From?.Username ?? "Anônimo",
                DataHora = DateTime.UtcNow,
                EnviadaPorAtendente = false
            };

            _ = Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine("Chamando CriarAtendimentoAsync...");
                    await _distribuidorService.CriarAtendimentoAsync(mensagem);
                    Console.WriteLine("Atendimento criado com sucesso!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao processar atendimento: " + ex.Message);
                }
            });

            return Ok();
        }

    }
}