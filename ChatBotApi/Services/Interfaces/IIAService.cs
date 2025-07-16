namespace ChatBotApi.Services.Interfaces
{
    public interface IIAService
    {
        Task<(string categoria, string titulo, string Resumo)> ClassificarMensagemAsync(string mensagem);
     
    }
}
