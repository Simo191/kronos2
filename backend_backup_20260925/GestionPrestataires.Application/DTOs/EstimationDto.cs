namespace GestionPrestataires.Application.DTOs;

public class EstimationDto
{
    public Guid Id { get; set; }
    public int Annee { get; set; }
    public decimal BudgetCagnotte { get; set; }
    public decimal BudgetConsomme { get; set; }
    public Guid CreePar { get; set; }
    public string? CreateurNom { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime DateModification { get; set; }
    public List<LigneEstimationDto> Lignes { get; set; } = new();
}

public class LigneEstimationDto
{
    public Guid Id { get; set; }
    public Guid EstimationId { get; set; }
    public Guid? PrestataireId { get; set; }
    public string? NomFuturPrestataire { get; set; }
    public string Mois { get; set; } = string.Empty;
    public decimal NombreJours { get; set; }
    public decimal Tjm { get; set; }
    public decimal ordre { get; set; }
    public decimal Revenu { get; set; }

    // ✅ Pour affichage
    public string NomPrestataire { get; set; } = string.Empty; // Soit prestataire.nom soit NomFuturPrestataire
}

public class CreateEstimationCommand
{
    public int Annee { get; set; }
    public decimal BudgetCagnotte { get; set; }
    public decimal BudgetConsomme { get; set; }
    public List<CreateLigneEstimationDto> Lignes { get; set; } = new();
}

public class CreateLigneEstimationDto
{
    public Guid? PrestataireId { get; set; }
    public string? NomFuturPrestataire { get; set; }
    public string Mois { get; set; } = string.Empty;
    public decimal NombreJours { get; set; }
    public decimal Tjm { get; set; }
}

public class UpdateEstimationCommand
{
    public decimal BudgetCagnotte { get; set; }
    public decimal BudgetConsomme { get; set; }
    public List<UpdateLigneEstimationDto> Lignes { get; set; } = new();
}

public class UpdateLigneEstimationDto
{
    public Guid? Id { get; set; } // Null si nouvelle ligne
    public Guid? PrestataireId { get; set; }
    public string? NomFuturPrestataire { get; set; }
    public string Mois { get; set; } = string.Empty;
    public decimal NombreJours { get; set; }
    public decimal Tjm { get; set; }
}