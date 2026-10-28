namespace GestionPrestataires.Domain.Entities;

/// <summary>
///   Serveur / environnement associé à une application planifiée.
///   Ex : VM de DEV X72SDEIGX6J, VM de PROD X72SDEIGX6K…
/// </summary>
public class ApplicationServer
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }

    /// <summary>Type d'environnement : DEV, RECETTE, PROD, INT, AUTRE.</summary>
    public string Type { get; set; } = "DEV";

    /// <summary>Identifiant / nom de la VM (ex : X72SDEIGX6J).</summary>
    public string Identifiant { get; set; } = string.Empty;

    /// <summary>Libellé facultatif (ex : VM principale, Front, DB).</summary>
    public string? Libelle { get; set; }

    /// <summary>Note libre (URL, hostname, port, IP…).</summary>
    public string? Note { get; set; }

    /// <summary>Ordre d'affichage dans la liste.</summary>
    public int Ordre { get; set; }

    // Navigation
    public ApplicationPlanning? Application { get; set; }
}
