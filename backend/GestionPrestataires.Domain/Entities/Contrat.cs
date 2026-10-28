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

    //   Suivi des commandes (tous facultatifs : les contrats existants restent valides)
    public string? NumeroCommande { get; set; }
    public string? ReferenceCtr { get; set; }
    public string Tranche { get; set; } = TranchesContrat.Ferme; // "Ferme" | "Option"
    public Guid? ApplicationId { get; set; }                      // Logiciel (application du planning)
    public string? Commentaire { get; set; }

    public Prestataire? Prestataire { get; set; }
    public TypeContrat? TypeContrat { get; set; }
    public ApplicationPlanning? Application { get; set; }
    public ICollection<ContratRepartitionAnnuelle> Repartitions { get; set; } = new List<ContratRepartitionAnnuelle>();
}

public static class TranchesContrat
{
    public const string Ferme = "Ferme";
    public const string Option = "Option";

    //   Toute valeur inconnue est ramenée à "Ferme" (comportement historique)
    public static string Normaliser(string? tranche) =>
        string.Equals(tranche?.Trim(), Option, StringComparison.OrdinalIgnoreCase) ? Option : Ferme;
}
