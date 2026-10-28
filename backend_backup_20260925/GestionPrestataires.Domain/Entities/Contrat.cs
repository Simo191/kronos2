namespace GestionPrestataires.Domain.Entities;

public class Contrat
{
    public Guid Id { get; set; }
    public Guid PrestataireId { get; set; }
    public Guid TypeContratId { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public decimal NombreJours { get; set; }
    public decimal Tjm { get; set; }
    public decimal MontantTotal { get; set; }
     public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public decimal? SeuilJoursMois { get; set; }
    public string? DocumentUrl { get; set; }
    public bool Active { get; set; } = true;

    public Prestataire? Prestataire { get; set; }
    public TypeContrat? TypeContrat { get; set; }
}
