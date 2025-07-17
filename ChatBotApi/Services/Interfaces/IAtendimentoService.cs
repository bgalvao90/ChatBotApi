using ChatBotApi.DTOs;
using ChatBotApi.Models;
using ChatBotApi.Models.Enums;

namespace ChatBotApi.Services.Interfaces
{
    public interface IAtendimentoService
    {
        Task<IEnumerable<Atendimento>> ObterAtendimentosAsync();
        Task<Atendimento?> ObterPorIdAsync(int id);
        Task<bool> StatusAtendimentoAsync(int id, AtendimentoStatus status);
        Task<bool> ResponderClienteAsync(Mensagem respostaDto);
        Task<bool> ResponderAtendenteAsync(Mensagem respostaDto);
        Task<bool> FinalizarAtendimentoAsync(int id);
        Task<bool> ResponderAtendimentoAsync(int id, int atendenteId, MensagemEntradaDto dto);
        Task<bool> TransferirAtendimentoAsync(int id, int paraAtendenteId);
        Task<bool> AssumirAtendimentoAsync(int id, Atendente atendente);
        Task<bool> RemoverAsync(int id);
        Task<List<Atendimento>> ListarPendentesAsync();
        Task<List<Atendimento>> ListarPendentesClienteAsync(int clienteId);
        Task<List<Atendimento>> ListarDoCliente(int clienteId);
        Task<List<Atendimento>> ListarDoAtendente(int atendenteId);
        Task<List<Atendimento>> FilaAtendimento();
        Task<List<Mensagem>> ListarMensagensAsync(int atendimentoId);
    }
}
