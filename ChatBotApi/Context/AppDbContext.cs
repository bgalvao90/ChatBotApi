using ChatBotApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatBotApi.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Atendimento>? Atendimentos { get; set; }
        public DbSet<Atendente>? Atendentes { get; set; }
        public DbSet<Mensagem>? Mensagens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Atendente>()
                .HasMany(a => a.Atendimentos)
                .WithOne()
                .HasForeignKey(a => a.AtendenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Atendimento>()
                .HasMany(a => a.Mensagens)
                .WithOne()
                .HasForeignKey(m => m.AtendimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mensagem>().ToTable("MensagemHistorico");

            modelBuilder.Entity<Atendente>()
                .Property(a => a.Nome)
                .HasColumnName("NomeCompleto");

            modelBuilder.Entity<Atendente>()
                .Property(a => a.Nome)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Atendente>()
                .Property(a => a.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Atendente>()
                .Property(a => a.Disponivel)
                .HasDefaultValue(true);

            modelBuilder.Entity<Atendimento>()
                .Property(a => a.Status)
                .HasConversion<string>();
        }

    }
}
