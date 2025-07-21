using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class chatHub : Hub
{
    public async Task EntrarNoGrupo(string atendimentoId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, atendimentoId);
    }
    public async Task Digitando(string atendimentoId, string nome)
    {
        await Clients.Group(atendimentoId).SendAsync("UsuarioDigitando", nome);
    }

    public override async Task OnDisconnectedAsync(System.Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
