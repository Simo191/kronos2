namespace GestionPrestataires.Application.DTOs;

public class ContratDto
{
    public Guid Id { get; set; }
    public Guid PrestataireId { get; set; }
    public Guid TypeContratId { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public decimal NombreJours { get; set; }
    public decimal Tjm { get; set; }
    public decimal MontantTotal { get; set; }
    public decimal? SeuilJoursMois { get; set; }

    //   NOUVELLE PROPRIÉTÉ : Jours utilisés calculés depuis les shifts
    public int JoursUtilises { get; set; }
    public string DocumentUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Active { get; set; }

    //   Suivi des commandes
    public string? NumeroCommande { get; set; }
    public string? ReferenceCtr { get; set; }
    public string Tranche { get; set; } = "Ferme";
    public Guid? ApplicationId { get; set; }
    public string? ApplicationNom { get; set; }
    public string? Commentaire { get; set; }
    //   Répartition enregistrée (vide = jamais saisie → proposition automatique côté suivi)
    public List<RepartitionAnnuelleDto> Repartitions { get; set; } = new();

    // Relations (optionnelles)
    public PrestataireDto? Prestataire { get; set; }
    public TypeContratDto? TypeContrat { get; set; }
}

public class RepartitionAnnuelleDto
{
    public int Annee { get; set; }
    public decimal NombreJours { get; set; }
}

// DTO Prestataire simplifié
public class PrestataireDto
{
    public Guid Id { get; set; }
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public decimal Tjm { get; set; }
    public string? ImageUrl { get; set; }
    //   Entreprise / SSII de rattachement (facultatif) — utilisé pour afficher (CODE)
    public Guid? EntrepriseId { get; set; }
    public PrestataireEntrepriseDto? Entreprise { get; set; }
}

//   Vue compacte de l'entreprise embarquée dans un PrestataireDto.
public class PrestataireEntrepriseDto
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Code { get; set; }
}

// DTO TypeContrat simplifié
public class TypeContratDto
{
    public Guid Id { get; set; }
    public string? Libelle { get; set; }
    public string? Description { get; set; }
}
