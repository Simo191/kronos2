namespace GestionPrestataires.Application.DTOs;

/// <summary>
/// DTO JourTravail RESTREINT pour les Users
/// TJM et Revenu sont MASQUÉS
/// </summary>
public class JourTravailDtoRestricted
{
    public Guid Id { get; set; }
    public Guid PrestataireId { get; set; }
    public string Mois { get; set; } = string.Empty;  // Format: "2025-12"
    public decimal NombreJours { get; set; }

    // ❌ TJM MASQUÉ
    // public decimal Tjm { get; set; }  // RETIRÉ

    // ❌ REVENU MASQUÉ
    // public decimal Revenu { get; set; }  // RETIRÉ

    public string? Description { get; set; }
    public DateTime DateTravail { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation RESTREINTE
    public PrestataireDtoSimple? Prestataire { get; set; }
}