namespace ChatBotApi.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        public int? UserModelId { get; set; }
        public UserModel? Usuario { get; set; }

        public ICollection<Atendimento>? Atendimentos { get; set; }
    }

}
