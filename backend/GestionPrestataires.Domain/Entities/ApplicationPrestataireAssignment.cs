namespace GestionPrestataires.Domain.Entities;

/// <summary>
/// Table de jointure Application ↔ Prestataire
/// </summary>
public class ApplicationPrestataireAssignment
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public Guid PrestataireId { get; set; }
    public string? Role { get; set; }

    // Navigation
    public ApplicationPlanning? Application { get; set; }
    public Prestataire? Prestataire { get; set; }
}