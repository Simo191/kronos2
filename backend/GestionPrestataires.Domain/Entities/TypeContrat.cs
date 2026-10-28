namespace GestionPrestataires.Domain.Entities;

public class TypeContrat
{
    public Guid Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<Contrat> Contrats { get; set; } = new List<Contrat>();
}
