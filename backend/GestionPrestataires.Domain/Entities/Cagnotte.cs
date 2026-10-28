namespace GestionPrestataires.Domain.Entities;

public class Cagnotte
{
    public Guid Id { get; set; }
    public int Annee { get; set; }
    public decimal BudgetAnnuel => SourcesBudget?.Sum(s => s.Montant) ?? 0;
    public decimal BudgetConsomme { get; set; }
    public decimal BudgetRestant => BudgetAnnuel - BudgetConsomme;
    public DateTime DateCreation { get; set; }
    public DateTime DateModification { get; set; }

    // Navigation
    public List<SourceBudget> SourcesBudget { get; set; } = new();
}