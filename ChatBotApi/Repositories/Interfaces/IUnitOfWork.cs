namespace ChatBotApi.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IAtendenteRepository AtendenteRepository { get; }
        IAtendimentoRepository AtendimentoRepository { get; }
        IMensagemRepository MensagemRepository { get; }

        Task CommitAsync();
    }
}
