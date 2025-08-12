using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ChatBotApi.Services.Interfaces;

public class IAService : IIAService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public IAService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();

        _apiKey = configuration["Groq:ApiKey"] ?? Environment.GetEnvironmentVariable("GROQ_API_KEY");

        if (string.IsNullOrEmpty(_apiKey))
            throw new Exception("A chave da Groq está vazia ou não foi encontrada.");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<(string categoria, string titulo, string Resumo)> ClassificarMensagemAsync(string mensagem)
    {
        var prompt = $@"
            Você é um assistente inteligente que ajuda atendentes a classificar mensagens de clientes.

            Com base na mensagem abaixo, retorne um JSON com:
            - ""Categoria"": Categoria principal do problema. Ex: Suporte - WD, Financeiro, Comercial
            - Suporte - 1 irá ser relacionado com ajuste de integração, integração, criação de usuário e divergência de dados.
            - Suporte - 2 irá ser relacionado com problemas ou duvidas sobre cotação, boleto bancário, homologações.
            - ""Titulo"": Frase curta com até 6 palavras resumindo o problema.
            - ""Resumo"": Um pequeno resumo da situação (até 2 frases).

            Mensagem: ""{mensagem}""

            Retorne apenas o JSON neste formato:
            {{ ""Categoria"": ""..."", ""Titulo"": ""..."", ""Resumo"": ""..."" }}
            ";


        var request = new
        {
            model = "meta-llama/llama-4-scout-17b-16e-instruct",
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erro da Groq: {response.StatusCode} - {responseJson}");
        }

        // Extrai o conteúdo da resposta da IA
        using JsonDocument doc = JsonDocument.Parse(responseJson);
        var respostaBruta = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        // Parsea o JSON retornado pela IA
        using JsonDocument resultadoJson = JsonDocument.Parse(respostaBruta!);
        var root = resultadoJson.RootElement;

        var categoria = root.TryGetProperty("Categoria", out var catProp)
            ? catProp.GetString()?.Trim() ?? "outros"
            : "outros";

        var titulo = root.TryGetProperty("Titulo", out var tituloProp)
            ? tituloProp.GetString()?.Trim() ?? "Novo atendimento"
            : "Novo atendimento";

        var resumo = root.TryGetProperty("Resumo", out var resumoProp)
            ? resumoProp.GetString()?.Trim() ?? ""
            : "";


        return (categoria, titulo, resumo);
    }
}
