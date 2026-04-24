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
    public DbSet<PointageIntervention> PointagesIntervention { get; set; }
    public DbSet<RapportIncident> RapportsIncidents { get; set; }
    public DbSet<DemandeFabrication> DemandesFabrication { get; set; }
    public DbSet<NatureTravail> NaturesTravail { get; set; }
    public DbSet<BonDeConsignation> BonsDeConsignation { get; set; }
    public DbSet<FicheEntretienPreventif> FichesEntretienPreventif { get; set; }
    public DbSet<TacheEntretien> TachesEntretien { get; set; }
    public DbSet<MatiereFabrication> MatieresFabrication { get; set; }
    public DbSet<IntervenantFabrication> IntervenantsFabrication { get; set; }
    public DbSet<PointageMachineFabrication> PointagesMachinesFabrication { get; set; }
    public DbSet<AppNotification> Notifications { get; set; }
    public DbSet<InterventionLog> InterventionLogs { get; set; }

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

        modelBuilder.Entity<PointageIntervention>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(p => p.InterventionRole)
                .WithMany(r => r.Pointages)
                .HasForeignKey(p => p.InterventionRoleId)
                .OnDelete(DeleteBehavior.Cascade);
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

        modelBuilder.Entity<DemandeFabrication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.NumeroFabrication).IsUnique();

            entity.HasOne(f => f.Equipement)
                .WithMany()
                .HasForeignKey(f => f.EquipementId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.DemandeTravail)
                .WithMany(d => d.DemandesFabrication)
                .HasForeignKey(f => f.DemandeTravailId)
                .OnDelete(DeleteBehavior.ClientSetNull);
                
            entity.Property(e => e.TotalCoutPieces).HasPrecision(18, 2);
            entity.Property(e => e.TotalCoutMainOeuvre).HasPrecision(18, 2);
            entity.Property(e => e.TotalCoutOperation).HasPrecision(18, 2);
        });

        modelBuilder.Entity<BonDeConsignation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(b => b.Equipement)
                .WithMany()
                .HasForeignKey(b => b.EquipementId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.DemandeTravail)
                .WithMany(d => d.BonsDeConsignation)
                .HasForeignKey(b => b.DemandeTravailId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(b => b.AgentConsignation)
                .WithMany()
                .HasForeignKey(b => b.AgentConsignationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.AgentDeconsignation)
                .WithMany()
                .HasForeignKey(b => b.AgentDeconsignationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FicheEntretienPreventif>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(f => f.Equipement)
                .WithMany()
                .HasForeignKey(f => f.EquipementId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.Intervenant)
                .WithMany()
                .HasForeignKey(f => f.IntervenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TacheEntretien>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(t => t.FicheEntretienPreventif)
                .WithMany(f => f.Taches)
                .HasForeignKey(t => t.FicheEntretienPreventifId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MatiereFabrication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Prix).HasPrecision(18, 2);
            entity.HasOne(m => m.DemandeFabrication)
                .WithMany(d => d.MatieresConsommees)
                .HasForeignKey(m => m.DemandeFabricationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IntervenantFabrication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CoutHoraire).HasPrecision(18, 2);
            entity.HasOne(i => i.DemandeFabrication)
                .WithMany(d => d.Intervenants)
                .HasForeignKey(i => i.DemandeFabricationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<PointageMachineFabrication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(p => p.DemandeFabrication)
                .WithMany(d => d.PointagesMachines)
                .HasForeignKey(p => p.DemandeFabricationId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(p => p.Intervenant)
                .WithMany()
                .HasForeignKey(p => p.IntervenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InterventionLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(l => l.DemandeTravail)
                .WithMany(d => d.JournalInterventions)
                .HasForeignKey(l => l.DemandeTravailId);
            
            entity.HasOne(l => l.Intervenant)
                .WithMany()
                .HasForeignKey(l => l.IntervenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
