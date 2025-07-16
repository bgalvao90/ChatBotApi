using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class OpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAIService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();

        _apiKey = configuration["Groq:ApiKey"] ?? Environment.GetEnvironmentVariable("GROQ_API_KEY");

        if (string.IsNullOrEmpty(_apiKey))
            throw new Exception("A chave da Groq está vazia ou não foi encontrada.");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<(string categoria, string titulo)> ClassificarMensagemAsync(string mensagem)
    {
        var prompt = $@"
Você é um assistente inteligente que ajuda atendentes humanos a entender rapidamente o tipo de problema de um cliente.

Com base na mensagem abaixo, retorne um JSON com:
- ""Categoria"": uma palavra como vendas, financeiro, suporte, técnico, entrega ou outros.
- ""Titulo"": uma frase curta de até 6 palavras, resumindo o problema de forma clara.

Mensagem: ""{mensagem}""

Apenas retorne o JSON no formato:
{{ ""Categoria"": ""..."", ""Titulo"": ""..."" }}
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
        var categoria = resultadoJson.RootElement.GetProperty("Categoria").GetString()?.Trim() ?? "outros";
        var titulo = resultadoJson.RootElement.GetProperty("Titulo").GetString()?.Trim() ?? "Novo atendimento";

        return (categoria, titulo);
    }
}
