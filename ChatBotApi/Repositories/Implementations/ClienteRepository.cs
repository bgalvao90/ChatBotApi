using ChatBotApi.Context;
using ChatBotApi.Models;
using ChatBotApi.Repositories.Interfaces;

namespace ChatBotApi.Repositories.Implementations
{
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        public ClienteRepository(AppDbContext context) : base(context)
    {
    }
}
}
