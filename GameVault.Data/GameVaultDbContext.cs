using GameVault.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Data;

public class GameVaultDbContext : DbContext
{
    public DbSet<Igra> Igre => Set<Igra>();
    public DbSet<Zanr> Zanrovi => Set<Zanr>();
    public DbSet<Platforma> Platforme => Set<Platforma>();

    public GameVaultDbContext(DbContextOptions<GameVaultDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Igra>(igra =>
        {
            igra.ToTable("Igre", tabela =>
            {
                tabela.HasCheckConstraint("CK_Igre_Naziv", "length(trim(Naziv)) > 0");
                tabela.HasCheckConstraint("CK_Igre_Ocena", "Ocena IS NULL OR Ocena BETWEEN 1 AND 10");
                tabela.HasCheckConstraint("CK_Igre_BrojSati", "BrojSati >= 0");
                tabela.HasCheckConstraint("CK_Igre_Status", "Status IN (0, 1, 2, 3)");
            });

            igra.HasMany(i => i.Zanrovi)
                .WithMany(z => z.Igre)
                .UsingEntity("IgraZanr");

            igra.HasMany(i => i.Platforme)
                .WithMany(p => p.Igre)
                .UsingEntity("IgraPlatforma");
        });

        modelBuilder.Entity<Zanr>(zanr =>
        {
            zanr.ToTable("Zanrovi", tabela =>
                tabela.HasCheckConstraint("CK_Zanrovi_Naziv", "length(trim(Naziv)) > 0"));
            zanr.HasIndex(z => z.Naziv).IsUnique();
        });

        modelBuilder.Entity<Platforma>(platforma =>
        {
            platforma.ToTable("Platforme", tabela =>
                tabela.HasCheckConstraint("CK_Platforme_Naziv", "length(trim(Naziv)) > 0"));
            platforma.HasIndex(p => p.Naziv).IsUnique();
        });
    }
}
