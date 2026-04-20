using Microsoft.EntityFrameworkCore;
using Sahel.GMAO.Core.Entities;

namespace Sahel.GMAO.Infrastructure.Data;

public class GmaoDbContext : DbContext
{
    public GmaoDbContext(DbContextOptions<GmaoDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Equipement> Equipements { get; set; }
    public DbSet<DemandeTravail> DemandesTravail { get; set; }
    public DbSet<ArticlePdr> ArticlesPdr { get; set; }
    public DbSet<ConsommableUsage> ConsommableUsages { get; set; }
    public DbSet<InterventionRole> InterventionRoles { get; set; }
    public DbSet<MaintenancePreventive> MaintenancePreventives { get; set; }
    public DbSet<RapportIncident> RapportsIncidents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
        });

        modelBuilder.Entity<Equipement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<DemandeTravail>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.NumeroDT).IsUnique();

            entity.HasOne(d => d.Demandeur)
                .WithMany()
                .HasForeignKey(d => d.DemandeurId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Equipement)
                .WithMany(e => e.DemandesTravail)
                .HasForeignKey(d => d.EquipementId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.Property(e => e.TotalCoutPieces).HasPrecision(18, 2);
            entity.Property(e => e.TotalCoutMainOeuvre).HasPrecision(18, 2);
            entity.Property(e => e.TotalCoutOperation).HasPrecision(18, 2);
        });

        modelBuilder.Entity<ArticlePdr>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CodeArticle).IsUnique();
            entity.Property(e => e.PrixUnitaire).HasPrecision(18, 2);
        });

        modelBuilder.Entity<ConsommableUsage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrixUnitaireApplique).HasPrecision(18, 2);

            entity.HasOne(u => u.DemandeTravail)
                .WithMany(d => d.Consommables)
                .HasForeignKey(u => u.DemandeTravailId);

            entity.HasOne(u => u.ArticlePdr)
                .WithMany(p => p.Usages)
                .HasForeignKey(u => u.ArticlePdrId);
        });

        modelBuilder.Entity<InterventionRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TauxHoraire).HasPrecision(18, 2);

            entity.HasOne(r => r.DemandeTravail)
                .WithMany(d => d.Intervenants)
                .HasForeignKey(r => r.DemandeTravailId);

            entity.HasOne(r => r.Intervenant)
                .WithMany()
                .HasForeignKey(r => r.IntervenantId);
        });

        modelBuilder.Entity<MaintenancePreventive>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(m => m.Equipement)
                .WithMany()
                .HasForeignKey(m => m.EquipementId);
        });

        modelBuilder.Entity<RapportIncident>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.NumeroRapport).IsUnique();

            entity.HasOne(r => r.Equipement)
                .WithMany()
                .HasForeignKey(r => r.EquipementId);

            entity.HasOne(r => r.Redacteur)
                .WithMany()
                .HasForeignKey(r => r.RedacteurId);
        });
    }
}
