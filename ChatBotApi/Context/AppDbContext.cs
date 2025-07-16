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
        public DbSet<Cliente>? Clientes { get; set; }
        public DbSet<UserModel>? Usuarios { get; set; }

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

            modelBuilder.Entity<UserModel>().
                HasOne(u => u.Atendente)
                .WithOne(a => a.Usuario)
                .HasForeignKey<Atendente>(a => a.UserModelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>().
                HasOne(u => u.Cliente)
                .WithOne(a => a.Usuario)
                .HasForeignKey<Cliente>(a => a.UserModelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>()
                 .Property(u => u.Role)
                 .HasConversion<string>();
        }

    }
}
