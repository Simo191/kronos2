namespace GestionPrestataires.Domain.Entities;

/// <summary>
///   Entreprise/SSII rattachée à un prestataire (ex: Capgemini, Atos, TCS…).
///   Gérée par les Admin/Manager via la page Paramétrage.
/// </summary>
public class Entreprise
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    /// <summary>Code court affiché entre parenthèses (ex: "CAP", "TCS"). Facultatif.</summary>
    public string? Code { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public ICollection<Prestataire> Prestataires { get; set; } = new List<Prestataire>();
}
