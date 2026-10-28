namespace GestionPrestataires.Domain.Entities;

public class Prestataire
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public decimal Tjm { get; set; }
    public string? ImageUrl { get; set; }    

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Responsable { get; set; } = false;
    public int? Order { get; set; } = 0;
    //   RELATION 1-1 AVEC USER (NULLABLE)
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Contrat> Contrats { get; set; } = new List<Contrat>();
    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
    public ICollection<JourTravail> JoursTravail { get; set; } = new List<JourTravail>();
    public ICollection<LigneEstimation> LigneEstimation { get; set; } = new List<LigneEstimation>();
 }
