namespace GestionPrestataires.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid RoleId { get; set; }

    //   ACTIVATION/DÉSACTIVATION
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public Role? Role { get; set; }

    //   RELATION 1-1 AVEC PRESTATAIRE (INVERSE)
    public Prestataire? Prestataire { get; set; }
    public ICollection<MoisVerrouille> MoisVerrouille { get; set; } = new List<MoisVerrouille>();
    public ICollection<Estimation> Estimation { get; set; } = new List<Estimation>();

}