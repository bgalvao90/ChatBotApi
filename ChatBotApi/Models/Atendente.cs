using ChatBotApi.Models.Enums;

namespace ChatBotApi.Models
{
    public class Atendente
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public string Funcao { get; set; }
        public bool Disponivel { get; set; }
        public AtendenteStatus Status { get; set; }
        public int UserModelId { get; set; }
        public UserModel? Usuario { get; set; }
        public ICollection<Atendimento> Atendimentos { get; set; } = new List<Atendimento>();
    }
}
