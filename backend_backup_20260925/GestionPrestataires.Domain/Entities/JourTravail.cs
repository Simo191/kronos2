namespace GestionPrestataires.Domain.Entities;

public class JourTravail
{
    public Guid Id { get; set; }
    public Guid PrestataireId { get; set; }
    public string Mois { get; set; } = string.Empty; // Format: "2025-12"
    public decimal NombreJours { get; set; }
    public decimal Tjm { get; set; }
    public string? Description { get; set; }
    public DateTime DateTravail { get; set; } // tjrs 01/mois/annee
    public DateTime CreatedAt { get; set; }

    public Prestataire? Prestataire { get; set; }
}
