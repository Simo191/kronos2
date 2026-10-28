namespace GestionPrestataires.Domain.Entities;

/// <summary>
/// Application/projet à planifier dans le Gantt
/// </summary>
public class ApplicationPlanning
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Statut { get; set; } = "Planifié"; // Planifié, En cours, Terminé, En pause
    public string Priorite { get; set; } = "Moyenne"; // Faible, Moyenne, Haute, Critique
    public string Couleur { get; set; } = "#3b82f6";
    public int Avancement { get; set; } = 0; // 0-100
    public DateTime DateCreation { get; set; }
    public DateTime DateModification { get; set; }
    public int Ordre { get; set; } = 0;
    // Navigation
    public ICollection<ApplicationPrestataireAssignment> Assignments { get; set; } = new List<ApplicationPrestataireAssignment>();

    //   Serveurs / environnements associés (DEV / RECETTE / PROD / INT / AUTRE)
    public ICollection<ApplicationServer> Servers { get; set; } = new List<ApplicationServer>();
}