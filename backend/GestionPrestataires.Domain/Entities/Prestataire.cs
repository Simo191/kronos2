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

    //   ACTIVATION/DÉSACTIVATION du compte prestataire
    //   Quand false : invisible dans tous les modules sauf la liste Prestataires (grisé)
    //                 + si un User est lié, sa connexion est bloquée
    public bool IsActive { get; set; } = true;

    //   Motif de la dernière désactivation (null si jamais désactivé ou réactivé depuis)
    public string? DeactivationReason { get; set; }

    //   Date de la dernière désactivation (null si jamais désactivé ou réactivé depuis)
    public DateTime? DeactivatedAt { get; set; }

    //   Entreprise / SSII de rattachement (facultative)
    public Guid? EntrepriseId { get; set; }
    public Entreprise? Entreprise { get; set; }

    //   RELATION 1-1 AVEC USER (NULLABLE)
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Contrat> Contrats { get; set; } = new List<Contrat>();
    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
    public ICollection<JourTravail> JoursTravail { get; set; } = new List<JourTravail>();
    public ICollection<LigneEstimation> LigneEstimation { get; set; } = new List<LigneEstimation>();
    //   Comptes techniques chiffrés (ADM, ADML, VPN…)
    public ICollection<PrestataireCredential> Credentials { get; set; } = new List<PrestataireCredential>();
}
