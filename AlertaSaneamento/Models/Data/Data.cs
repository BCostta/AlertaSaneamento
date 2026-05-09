using AlertaSaneamento.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlertaSaneamento.Models.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<Atualizacao> Atualizacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cidadao -> Alertas (um usuário tem muitos relatos)
            modelBuilder.Entity<Alerta>()
                .HasOne(a => a.Usuario)
                .WithMany(u => u.Alertas)
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Fiscal -> Atualizacoes (um fiscal envia muitas atualizações)
            modelBuilder.Entity<Atualizacao>()
                .HasOne(at => at.Fiscal)
                .WithMany(u => u.Atualizacoes)
                .HasForeignKey(at => at.FiscalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Alerta -> Atualizacoes (um relato tem muitas atualizações)
            modelBuilder.Entity<Atualizacao>()
                .HasOne(at => at.Alerta)
                .WithMany(a => a.Atualizacoes)
                .HasForeignKey(at => at.AlertaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Email único por usuário
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
