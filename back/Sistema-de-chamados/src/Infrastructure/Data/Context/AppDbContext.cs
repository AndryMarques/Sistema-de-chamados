using Microsoft.EntityFrameworkCore;
using Sistema_de_chamados.src.Domain.Entities;

namespace Sistema_de_chamados.src.Infrastructure.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Responsavel> Responsaveis { get; set; }
        public DbSet<Chamado> Chamados { get; set; }
        public DbSet<Acompanhamento> Acompanhamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Senha).IsRequired();
                entity.Property(e => e.Telefone).HasMaxLength(20);
            });

            // Configurações de Responsavel
            modelBuilder.Entity<Responsavel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.ResponsaveisAssociados)
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurações de Chamado
            modelBuilder.Entity<Chamado>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Descricao).IsRequired();
                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.ChamadosAbertos)
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Responsavel)
                    .WithMany(r => r.ChamadosAtribuidos)
                    .HasForeignKey(e => e.ResponsavelId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configurações de Acompanhamento
            modelBuilder.Entity<Acompanhamento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Descricao).IsRequired();
                entity.HasOne(e => e.Chamado)
                    .WithMany(c => c.Acompanhamentos)
                    .HasForeignKey(e => e.ChamadoId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Responsavel)
                    .WithMany()
                    .HasForeignKey(e => e.ResponsavelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
