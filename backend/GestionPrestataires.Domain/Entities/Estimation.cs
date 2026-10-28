namespace GestionPrestataires.Domain.Entities;

/// <summary>
/// En-tête d'une estimation budgétaire
/// </summary>
public class Estimation
{
    public Guid Id { get; set; }
    public int Annee { get; set; }
    public decimal BudgetCagnotte { get; set; }
    public decimal BudgetConsomme { get; set; }
    public Guid CreePar { get; set; } // Admin qui a créé
    public DateTime DateCreation { get; set; }
    public DateTime DateModification { get; set; }
    public Guid? ModifiePar { get; set; } // Admin qui a modifié en dernier


    // Navigation
    public User? Createur { get; set; }
    public User? Modificateur { get; set; }

    public ICollection<LigneEstimation> Lignes { get; set; } = new List<LigneEstimation>();
}