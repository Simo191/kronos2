namespace GestionPrestataires.Application.Contrats.Queries.GetSuiviCommandes;

public class SuiviCommandesDto
{
    //   Toutes les applications du planning (y compris celles sans commande)
    public List<SuiviApplicationDto> Applications { get; set; } = new();
    public List<SuiviCommandeLigneDto> Lignes { get; set; } = new();
}

public class SuiviApplicationDto
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Couleur { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public int Ordre { get; set; }
}

public class SuiviCommandeLigneDto
{
    public Guid ContratId { get; set; }
    //   Logiciel effectif : celui du contrat, sinon déduit des affectations du planning
    public Guid? ApplicationId { get; set; }
    public string? ApplicationNom { get; set; }
    public string ApplicationSource { get; set; } = SourcesLogiciel.Aucun;   // "contrat" | "planning" | "aucun"
    //   Logiciels où le prestataire est affecté dans le planning (ordre du planning)
    public List<SuiviLogicielDto> LogicielsPlanning { get; set; } = new();

    public string? NumeroCommande { get; set; }
    public string Tranche { get; set; } = "Ferme";
    public string? ReferenceCtr { get; set; }
    public string? Commentaire { get; set; }
    public string? TypeContrat { get; set; }
    public bool Active { get; set; }

    public Guid PrestataireId { get; set; }
    public string PrestataireNom { get; set; } = string.Empty;
    public string? PrestataireImageUrl { get; set; }
    public bool PrestataireActif { get; set; }
    public string? EntrepriseNom { get; set; }
    public string? EntrepriseCode { get; set; }

    public decimal Tjm { get; set; }
    public decimal NombreJours { get; set; }
    public decimal MontantTotal { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }

    //   true = aucune répartition saisie → proposition automatique (prorata jours ouvrés)
    public bool RepartitionAuto { get; set; }
    public List<SuiviAnneeDto> Annees { get; set; } = new();

    //   Consommation réelle (jours travaillés saisis sur la période du contrat)
    public decimal JoursConsommes { get; set; }
    public decimal MontantConsomme { get; set; }
    public decimal JoursRestants { get; set; }
    public decimal MontantRestant { get; set; }
}

public class SuiviLogicielDto
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
}

public static class SourcesLogiciel
{
    public const string Contrat = "contrat";
    public const string Planning = "planning";
    public const string Aucun = "aucun";
}

public class SuiviAnneeDto
{
    public int Annee { get; set; }
    public decimal JoursPrevus { get; set; }
    public decimal MontantPrevu { get; set; }
    public decimal JoursConsommes { get; set; }
    public decimal MontantConsomme { get; set; }
}
