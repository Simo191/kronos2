namespace GestionPrestataires.Domain.Entities;
/// <summary>
/// Entité pour verrouiller un mois
/// Empêche les Users de créer/modifier/supprimer des jours travail pour ce mois
/// </summary>
public class MoisVerrouille
{
    public Guid Id { get; set; }
    public string Mois { get; set; } = string.Empty; // Format: "2025-12"
    public Guid VerrouilleParUserId { get; set; } // Admin/Manager qui a verrouillé
    public DateTime DateVerrouillage { get; set; }
    public string? Commentaire { get; set; }

    // Navigation
    public User? VerrouilleParUser { get; set; }
}
