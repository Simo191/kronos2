namespace GestionPrestataires.Application.DTOs;

/// <summary>
/// DTO JourTravail COMPLET pour Admin/Manager
/// TJM et Revenu VISIBLES
/// </summary>
public class JourTravailDto
{
    public Guid Id { get; set; }
    public Guid PrestataireId { get; set; }
    public string Mois { get; set; } = string.Empty;  // Format: "2025-12"
    public decimal NombreJours { get; set; }

    //   TJM (comme dans l'entité, pas TauxJournalier)
    public decimal Tjm { get; set; }  //   VISIBLE pour Admin/Manager

    //   REVENU CALCULÉ (NombreJours * Tjm)
    public decimal Revenu { get; set; }  //   VISIBLE pour Admin/Manager

    public string? Description { get; set; }
    public DateTime DateTravail { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public PrestataireDtoSimple? Prestataire { get; set; }
}

/// <summary>
/// DTO Prestataire simplifié pour JourTravail
/// </summary>
public class PrestataireDtoSimple
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    //   Entreprise / SSII de rattachement (info non sensible)
    public Guid? EntrepriseId { get; set; }
    public PrestataireEntrepriseDto? Entreprise { get; set; }
}