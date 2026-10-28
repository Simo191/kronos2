namespace GestionPrestataires.Application.DTOs;

/// <summary>
/// DTO Contrat RESTREINT pour les Users
/// TJM et MontantTotal sont masqués
/// </summary>
public class ContratDtoRestricted
{
    public Guid Id { get; set; }
    public Guid PrestataireId { get; set; }
    public Guid TypeContratId { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public decimal NombreJours { get; set; }

    //   TJM MASQUÉ - Non inclus dans ce DTO
    // public decimal Tjm { get; set; }  // ❌ RETIRÉ

    //   MONTANT MASQUÉ - Non inclus dans ce DTO
    // public decimal MontantTotal { get; set; }  // ❌ RETIRÉ

    public decimal JoursUtilises { get; set; }
    public decimal? SeuilJoursMois { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Active { get; set; }

    //   CORRECTION : Utiliser PrestataireDtoRestricted au lieu de PrestataireDto
    public PrestataireDtoRestricted? Prestataire { get; set; }
    public TypeContratDto? TypeContrat { get; set; }
}

/// <summary>
/// DTO Prestataire RESTREINT pour les Users
/// TJM masqué
/// </summary>
public class PrestataireDtoRestricted
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public bool? Responsable { get; set; }
    public int? Order { get; set; }
    public string? ImageUrl { get; set; }
    //   TJM MASQUÉ
    // public decimal Tjm { get; set; }  // ❌ RETIRÉ

    public Guid? UserId { get; set; }

    //   Entreprise / SSII de rattachement (info non sensible)
    public Guid? EntrepriseId { get; set; }
    public PrestataireEntrepriseDto? Entreprise { get; set; }
}