using Microsoft.EntityFrameworkCore;
using SistemaConsultasUVV.Models;

namespace SistemaConsultasUVV.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Consulta> Consultas => Set<Consulta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .Property(u => u.DataCadastro)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Consulta>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Consultas)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
