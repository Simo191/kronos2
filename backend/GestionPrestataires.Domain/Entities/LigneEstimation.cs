namespace GestionPrestataires.Domain.Entities;

/// <summary>
/// Ligne d'estimation pour un prestataire (existant ou futur)
/// </summary>
public class LigneEstimation
{
    public Guid Id { get; set; }
    public Guid EstimationId { get; set; }

    // ✅ Prestataire existant (optionnel)
    public Guid? PrestataireId { get; set; }

    // ✅ OU nom d'un futur prestataire (optionnel)
    public string? NomFuturPrestataire { get; set; }

    public string Mois { get; set; } = string.Empty; // Format: "2025-12"
    public decimal NombreJours { get; set; }
    public decimal Tjm { get; set; }
    public decimal ordre { get; set; }
    public decimal Revenu { get; set; } // Calculé: NombreJours * Tjm

    // Navigation
    public Estimation? Estimation { get; set; }
    public Prestataire? Prestataire { get; set; }
}