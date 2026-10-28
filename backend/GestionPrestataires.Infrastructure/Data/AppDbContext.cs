using GestionPrestataires.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private static readonly DateTime SeedCreatedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Prestataire> Prestataires { get; set; }
    public DbSet<TypeContrat> TypesContrat { get; set; }
    public DbSet<Contrat> Contrats { get; set; }
    public DbSet<ContratRepartitionAnnuelle> ContratRepartitions { get; set; }
    public DbSet<JourTravail> JoursTravail { get; set; }
    public DbSet<Cagnotte> Cagnottes { get; set; }
    public DbSet<SourceBudget> SourceBudgets { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<TypeShift> TypeShifts { get; set; }
    public DbSet<MoisVerrouille> MoisVerrouilles { get; set; }
    public DbSet<Estimation> Estimations { get; set; }
    public DbSet<LigneEstimation> LigneEstimations { get; set; }
    public DbSet<ApplicationPlanning> ApplicationsPlannings { get; set; }
    public DbSet<ApplicationPrestataireAssignment> ApplicationsPrestataireAssignments { get; set; }
    public DbSet<ApplicationServer> ApplicationServers { get; set; }
    public DbSet<PrestataireCredential> PrestataireCredentials { get; set; }
    public DbSet<Entreprise> Entreprises { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Prestataire>(entity =>
        {
            entity.ToTable("prestataires");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nom).HasColumnName("nom").IsRequired();
            entity.Property(e => e.Prenom).HasColumnName("prenom").IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").IsRequired();
            entity.Property(e => e.Responsable).HasColumnName("responsable").HasDefaultValue(false);
            entity.Property(e => e.Order).HasColumnName("order").HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.DeactivationReason).HasColumnName("deactivation_reason").HasMaxLength(500);
            entity.Property(e => e.DeactivatedAt).HasColumnName("deactivated_at").HasColumnType("timestamp with time zone");
            entity.Property(e => e.EntrepriseId).HasColumnName("entreprise_id");
            entity.HasOne(e => e.Entreprise)
                  .WithMany(en => en.Prestataires)
                  .HasForeignKey(e => e.EntrepriseId)
                  .OnDelete(DeleteBehavior.SetNull);   //   suppression entreprise = on dérattache
            entity.Property(e => e.Telephone).HasColumnName("telephone");
            entity.Property(e => e.Tjm).HasColumnName("tjm").HasColumnType("numeric(10,2)");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");

            //   RELATION 1-1 AVEC USER (NULLABLE)
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.HasOne(e => e.User)
                .WithOne(u => u.Prestataire)
                .HasForeignKey<Prestataire>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict); //   Empêche suppression cascade
        });

        modelBuilder.Entity<TypeContrat>(entity =>
        {
            entity.ToTable("types_contrat");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Libelle).HasColumnName("libelle").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<Contrat>(entity =>
        {
            entity.ToTable("contrats");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PrestataireId).HasColumnName("prestataire_id");
            entity.Property(e => e.TypeContratId).HasColumnName("type_contrat_id");
            entity.Property(e => e.DateDebut).HasColumnName("date_debut").HasColumnType("date");
            entity.Property(e => e.DateFin).HasColumnName("date_fin").HasColumnType("date");
            entity.Property(e => e.NombreJours).HasColumnName("nombre_jours");
            entity.Property(e => e.Active).HasColumnName("active").HasDefaultValue(true);
            entity.Property(e => e.Tjm).HasColumnName("tjm").HasColumnType("numeric(10,2)");
            entity.Property(e => e.MontantTotal).HasColumnName("montant_total").HasColumnType("numeric(12,2)");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");

            //   Suivi des commandes
            entity.Property(e => e.NumeroCommande).HasColumnName("numero_commande").HasMaxLength(100);
            entity.Property(e => e.ReferenceCtr).HasColumnName("reference_ctr").HasMaxLength(100);
            entity.Property(e => e.Tranche).HasColumnName("tranche").HasMaxLength(10).IsRequired().HasDefaultValue(TranchesContrat.Ferme);
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.Commentaire).HasColumnName("commentaire").HasMaxLength(1000);

            entity.HasOne(e => e.Prestataire)
                .WithMany(p => p.Contrats)
                .HasForeignKey(e => e.PrestataireId);

            entity.HasOne(e => e.TypeContrat)
                .WithMany(t => t.Contrats)
                .HasForeignKey(e => e.TypeContratId);

            //   Suppression d'une application du planning = le contrat perd juste son logiciel
            entity.HasOne(e => e.Application)
                .WithMany()
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.ApplicationId).HasDatabaseName("ix_contrats_application_id");
        });

        modelBuilder.Entity<ContratRepartitionAnnuelle>(entity =>
        {
            entity.ToTable("contrat_repartitions_annuelles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContratId).HasColumnName("contrat_id").IsRequired();
            entity.Property(e => e.Annee).HasColumnName("annee").IsRequired();
            entity.Property(e => e.NombreJours).HasColumnName("nombre_jours").HasColumnType("numeric(10,2)").IsRequired();

            entity.HasOne(e => e.Contrat)
                  .WithMany(c => c.Repartitions)
                  .HasForeignKey(e => e.ContratId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ContratId, e.Annee })
                  .IsUnique()
                  .HasDatabaseName("ux_contrat_repartitions_contrat_annee");
        });

        modelBuilder.Entity<JourTravail>(entity =>
        {
            entity.ToTable("jours_travail");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PrestataireId).HasColumnName("prestataire_id");
            entity.Property(e => e.Tjm).HasColumnName("tjm").HasColumnType("numeric(10,2)");
            entity.Property(e => e.Mois).HasColumnName("Mois");
            entity.Property(e => e.NombreJours).HasColumnName("NombreJours").HasColumnType("numeric(10,2)");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone");
            entity.Property(e => e.DateTravail).HasColumnName("DateTravail").HasColumnType("date");

            entity.HasOne(e => e.Prestataire)
                .WithMany(p => p.JoursTravail)
                .HasForeignKey(e => e.PrestataireId);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email").IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
            entity.Property(e => e.FirstName).HasColumnName("first_name").IsRequired();
            entity.Property(e => e.LastName).HasColumnName("last_name").IsRequired();
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone");
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at").HasColumnType("timestamp with time zone");

            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId);
        });

        modelBuilder.Entity<Cagnotte>(entity =>
        {
            entity.ToTable("cagnottes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Annee).HasColumnName("annee").IsRequired();
            entity.Ignore(e => e.BudgetAnnuel);
            entity.Ignore(e => e.BudgetRestant);
            entity.Property(e => e.BudgetConsomme).HasColumnName("budget_consomme").HasColumnType("numeric(15,2)");
            entity.Property(e => e.DateCreation).HasColumnName("date_creation").HasColumnType("timestamp with time zone");
            entity.Property(e => e.DateModification).HasColumnName("date_modification").HasColumnType("timestamp with time zone");
            entity.HasIndex(e => e.Annee).IsUnique();
        });

        modelBuilder.Entity<SourceBudget>(entity =>
        {
            entity.ToTable("sources_budget");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CagnotteId).HasColumnName("cagnotte_id").IsRequired();
            entity.Property(e => e.Nom).HasColumnName("nom").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Montant).HasColumnName("montant").HasColumnType("numeric(15,2)").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(1000);
            entity.Property(e => e.DateCreation).HasColumnName("date_creation").HasColumnType("timestamp with time zone");
            entity.Property(e => e.DateModification).HasColumnName("date_modification").HasColumnType("timestamp with time zone");

            // Index pour optimiser les requêtes
            entity.HasIndex(e => e.CagnotteId).HasDatabaseName("ix_sources_budget_cagnotte_id");
            entity.HasIndex(e => e.Code).HasDatabaseName("ix_sources_budget_code");
        });
        modelBuilder.Entity<TypeShift>(entity =>
        {
            entity.ToTable("type_shifts");

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id)
                .HasColumnName("id")
                .IsRequired();

            entity.Property(t => t.Label)
                .HasColumnName("label")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(t => t.ShortLabel)
                .HasColumnName("short_label")
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(t => t.Code)
                .HasColumnName("code")
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(t => t.Code)
                .IsUnique()
                .HasDatabaseName("ix_type_shifts_code");

            entity.Property(t => t.Color)
                .HasColumnName("color")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(t => t.BgColor)
                .HasColumnName("bg_color")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(t => t.BorderColor)
                .HasColumnName("border_color")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(t => t.TextColor)
                .HasColumnName("text_color")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(t => t.IsActive)
                .HasColumnName("is_active")
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(t => t.Order)
                .HasColumnName("order")
                .IsRequired();

            //   Configuration de la colonne Category
            entity.Property(t => t.Category)
                .HasColumnName("category")
                .HasColumnType("integer")
                .IsRequired()
                .HasDefaultValue(ShiftCategory.Travail)
                .HasComment("0=Travail, 1=Absence, 2=Congé");

            entity.Property(t => t.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(t => t.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            // Index sur Category pour les requêtes de statistiques
            entity.HasIndex(t => t.Category)
                .HasDatabaseName("ix_type_shifts_category");
        });
        modelBuilder.Entity<Shift>(entity =>
        {
            entity.ToTable("shifts");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            //   FK PrestataireId (1 seule colonne)
            entity.Property(e => e.PrestataireId)
                .HasColumnName("prestataire_id") // <-- IMPORTANT (ou garde "prestataireid" si ta DB est déjà comme ça)
                .IsRequired();

            entity.Property(e => e.Date)
                .HasColumnName("date")
                .HasColumnType("date")
                .IsRequired();

            entity.Property(e => e.TypeShiftId)
                .HasColumnName("type_shift_id")
                .IsRequired();

            entity.Property(e => e.Notes)
                .HasColumnName("notes")
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            //   Relation TypeShift (liée à la navigation)
            entity.HasOne(s => s.TypeShift)
                .WithMany()
                .HasForeignKey(s => s.TypeShiftId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_shifts_type_shift_id");

            //   Relation Prestataire (liée à la navigation) => PLUS DE PrestataireId1
            entity.HasOne(s => s.Prestataire)
                .WithMany(p => p.Shifts)
                .HasForeignKey(s => s.PrestataireId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_shifts_prestataire_id");

            // Index
            entity.HasIndex(e => new { e.PrestataireId, e.Date })
                .HasDatabaseName("ix_shifts_prestataire_id_date");

            entity.HasIndex(e => e.TypeShiftId)
                .HasDatabaseName("ix_shifts_type_shift_id");

            entity.HasIndex(e => e.Date)
                .HasDatabaseName("ix_shifts_date");
        });

        modelBuilder.Entity<MoisVerrouille>(entity =>
        {
            entity.ToTable("mois_verrouilles");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Mois).HasColumnName("mois").HasMaxLength(7).IsRequired();
            entity.Property(e => e.VerrouilleParUserId).HasColumnName("verrouille_par_user_id").IsRequired();
            entity.Property(e => e.DateVerrouillage).HasColumnName("date_verrouillage").HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(e => e.Commentaire).HasColumnName("commentaire");

            entity.HasIndex(e => e.Mois).IsUnique();

            entity.HasOne(e => e.VerrouilleParUser)
                  .WithMany()
                  .HasForeignKey(e => e.VerrouilleParUserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Estimation>(entity =>
        {
            entity.ToTable("estimations");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Annee).HasColumnName("annee").IsRequired();
            entity.Property(e => e.BudgetCagnotte).HasColumnName("budget_cagnotte").HasColumnType("numeric(15,2)").IsRequired();
            entity.Property(e => e.BudgetConsomme).HasColumnName("budget_consomme").HasColumnType("numeric(15,2)").IsRequired();
            entity.Property(e => e.CreePar).HasColumnName("cree_par").IsRequired();
            entity.Property(e => e.DateCreation).HasColumnName("date_creation").HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(e => e.DateModification).HasColumnName("date_modification").HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(e => e.ModifiePar).HasColumnName("modifie_par").IsRequired(false);

            entity.HasIndex(e => e.Annee).IsUnique();

            entity.HasOne(e => e.Createur)
                  .WithMany()
                  .HasForeignKey(e => e.CreePar)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Modificateur)
                  .WithMany()
                  .HasForeignKey(e => e.ModifiePar)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(false);

            entity.HasMany(e => e.Lignes)
                  .WithOne(l => l.Estimation)
                  .HasForeignKey(l => l.EstimationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration LigneEstimation
        modelBuilder.Entity<LigneEstimation>(entity =>
        {
            entity.ToTable("lignes_estimation");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EstimationId).HasColumnName("estimation_id").IsRequired();
            entity.Property(e => e.PrestataireId).HasColumnName("prestataire_id");
            entity.Property(e => e.NomFuturPrestataire).HasColumnName("nom_futur_prestataire").HasMaxLength(200);
            entity.Property(e => e.Mois).HasColumnName("mois").HasMaxLength(7).IsRequired();
            entity.Property(e => e.NombreJours).HasColumnName("nombre_jours").HasColumnType("numeric(10,2)").IsRequired();
            entity.Property(e => e.Tjm).HasColumnName("tjm").HasColumnType("numeric(15,2)").IsRequired();
            entity.Property(e => e.Revenu).HasColumnName("revenu").HasColumnType("numeric(15,2)").IsRequired();

            entity.HasIndex(e => new { e.EstimationId, e.Mois });

            entity.HasOne(e => e.Prestataire)
                  .WithMany()
                  .HasForeignKey(e => e.PrestataireId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ApplicationPlanning>(entity =>
        {
            entity.ToTable("applications_planning");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nom).HasColumnName("nom").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DateDebut).HasColumnName("date_debut").HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(e => e.DateFin).HasColumnName("date_fin").HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(e => e.Statut).HasColumnName("statut").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Priorite).HasColumnName("priorite").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Couleur).HasColumnName("couleur").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Avancement).HasColumnName("avancement").IsRequired();
            entity.Property(e => e.DateCreation).HasColumnName("date_creation").HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(e => e.DateModification).HasColumnName("date_modification").HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(e => e.Ordre).HasColumnName("ordre").HasDefaultValue(0);
            entity.HasMany(e => e.Assignments)
                  .WithOne(a => a.Application)
                  .HasForeignKey(a => a.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApplicationPrestataireAssignment>(entity =>
        {
            entity.ToTable("application_prestataire_assignments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id").IsRequired();
            entity.Property(e => e.PrestataireId).HasColumnName("prestataire_id").IsRequired();
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(100);

            entity.HasOne(e => e.Prestataire)
                  .WithMany()
                  .HasForeignKey(e => e.PrestataireId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        //   Entreprises / SSII de rattachement des prestataires
        modelBuilder.Entity<Entreprise>(entity =>
        {
            entity.ToTable("entreprises");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nom).HasColumnName("nom").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(20);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

            entity.HasIndex(e => e.Nom).HasDatabaseName("ix_entreprises_nom");
        });

        //   Comptes techniques (ADM, ADML, VPN…) d'un prestataire — chiffrés AES-256-GCM
        modelBuilder.Entity<PrestataireCredential>(entity =>
        {
            entity.ToTable("prestataire_credentials");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PrestataireId).HasColumnName("prestataire_id").IsRequired();
            entity.Property(e => e.Label).HasColumnName("label").HasMaxLength(50).IsRequired();
            entity.Property(e => e.AccountName).HasColumnName("account_name").HasMaxLength(200).IsRequired();
            entity.Property(e => e.EncryptedPassword).HasColumnName("encrypted_password").HasMaxLength(2000).IsRequired();
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at").HasColumnType("timestamp with time zone");
            entity.Property(e => e.Note).HasColumnName("note").HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

            entity.HasOne(e => e.Prestataire)
                  .WithMany(p => p.Credentials)
                  .HasForeignKey(e => e.PrestataireId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.PrestataireId).HasDatabaseName("ix_prestataire_credentials_prestataire_id");
        });

        //   Serveurs / environnements d'une application planifiée
        modelBuilder.Entity<ApplicationServer>(entity =>
        {
            entity.ToTable("application_servers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id").IsRequired();
            entity.Property(e => e.Type).HasColumnName("type").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Identifiant).HasColumnName("identifiant").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Libelle).HasColumnName("libelle").HasMaxLength(200);
            entity.Property(e => e.Note).HasColumnName("note").HasMaxLength(1000);
            entity.Property(e => e.Ordre).HasColumnName("ordre").HasDefaultValue(0);

            entity.HasOne(e => e.Application)
                  .WithMany(a => a.Servers)
                  .HasForeignKey(e => e.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ApplicationId).HasDatabaseName("ix_application_servers_application_id");
        });

        var seedCreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var typeShifts = new[]
            {
                // ============ TRAVAIL ============
                new TypeShift
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Label = "Sur Site",
                    ShortLabel = "Sur Site",
                    Code = "SUR_SITE",
                    Color = "#4ade80",
                    BgColor = "bg-green-200",
                    BorderColor = "border-green-400",
                    TextColor = "text-green-900",
                    Order = 1,
                    Category = ShiftCategory.Travail, //   Travail
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TypeShift
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Label = "Télétravail",
                    ShortLabel = "Télétrav.",
                    Code = "TELETRAVAIL",
                    Color = "#facc15",
                    BgColor = "bg-yellow-200",
                    BorderColor = "border-yellow-400",
                    TextColor = "text-yellow-900",
                    Order = 2,
                    Category = ShiftCategory.Travail, //   Travail
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TypeShift
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Label = "Formation",
                    ShortLabel = "Formatio.",
                    Code = "FORMATION",
                    Color = "#60a5fa",
                    BgColor = "bg-blue-200",
                    BorderColor = "border-blue-400",
                    TextColor = "text-blue-900",
                    Order = 3,
                    Category = ShiftCategory.Travail, //   Travail
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TypeShift
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Label = "Déplacement",
                    ShortLabel = "Déplacmt.",
                    Code = "DEPLACEMENT",
                    Color = "#c084fc",
                    BgColor = "bg-purple-200",
                    BorderColor = "border-purple-400",
                    TextColor = "text-purple-900",
                    Order = 4,
                    Category = ShiftCategory.Travail, //   Travail
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // ============ ABSENCE ============
                new TypeShift
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Label = "Absence",
                    ShortLabel = "Absence",
                    Code = "ABSENCE",
                    Color = "#9ca3af",
                    BgColor = "bg-gray-200",
                    BorderColor = "border-gray-400",
                    TextColor = "text-gray-900",
                    Order = 5,
                    Category = ShiftCategory.Absence, //   Absence
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // ============ CONGÉ ============
                new TypeShift
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Label = "Congé",
                    ShortLabel = "Congé",
                    Code = "CONGE",
                    Color = "#f87171",
                    BgColor = "bg-red-200",
                    BorderColor = "border-red-400",
                    TextColor = "text-red-900",
                    Order = 6,
                    Category = ShiftCategory.Conge, //   Congé
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TypeShift
                {
                    Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Label = "École",
                    ShortLabel = "École",
                    Code = "ECOLE",
                    Color = "#fb923c",
                    BgColor = "bg-orange-200",
                    BorderColor = "border-orange-400",
                    TextColor = "text-orange-900",
                    Order = 7,
                    Category = ShiftCategory.Conge, //   Congé
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TypeShift
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    Label = "Séminaire",
                    ShortLabel = "Séminair.",
                    Code = "SEMINAIRE",
                    Color = "#047857",
                    BgColor = "bg-emerald-700",
                    BorderColor = "border-emerald-800",
                    TextColor = "text-white",
                    Order = 8,
                    Category = ShiftCategory.Travail, //   Travail
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

        modelBuilder.Entity<TypeShift>().HasData(typeShifts);
        var roles = new[]
        {
                new Role { Id = SeedIds.Role_Admin,   Name = "Admin",   DisplayName = "Administrateur", Description = "Full system access",  CreatedAt = SeedCreatedAt },
                new Role { Id = SeedIds.Role_Manager, Name = "Manager", DisplayName = "Manager",       Description = "Management access",  CreatedAt = SeedCreatedAt },
                new Role {Id = SeedIds.Role_User, Name = "User", DisplayName = "User", Description = "Basic user access", CreatedAt = SeedCreatedAt}
            };
        modelBuilder.Entity<Role>().HasData(roles);

        // ===== Seed Admin User =====
        var adminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var adminUser = new User
        {
            Id = adminUserId,
            RoleId = SeedIds.Role_Admin,
            FirstName = "System",
            LastName = "Administrateur",
            Email = "admin@LeaStudio.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            IsActive = true,
            CreatedAt = SeedCreatedAt
        };
        modelBuilder.Entity<User>().HasData(adminUser);

        // ===== Seed Manager User =====
        var managerUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var managerUser = new User
        {
            Id = managerUserId,
            RoleId = SeedIds.Role_Manager,
            FirstName = "System",
            LastName = "Manager",
            Email = "razqani@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Med123!"),
            IsActive = true,
            CreatedAt = SeedCreatedAt
        };
        modelBuilder.Entity<User>().HasData(managerUser);

        // ===== Seed User User =====
        var userId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var userUser = new User
        {
            Id = userId,
            FirstName = "System",
            RoleId = SeedIds.Role_User,
            LastName = "User",
            Email = "rs.razqani@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Raz123!"),
            IsActive = true,
            CreatedAt = SeedCreatedAt
        };
        modelBuilder.Entity<User>().HasData(userUser);
    }
}
