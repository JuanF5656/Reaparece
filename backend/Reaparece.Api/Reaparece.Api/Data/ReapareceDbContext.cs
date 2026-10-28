
using Microsoft.EntityFrameworkCore;
using Reaparece.Api.Models;

namespace Reaparece.Api.Data;

public class ReapareceDbContext : DbContext
{
    public ReapareceDbContext(
        DbContextOptions<ReapareceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Correo)
                .HasMaxLength(254)
                .IsRequired();

            entity.HasIndex(u => u.Correo)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired();
        });
    }
}

