using MediatR;

namespace GestionPrestataires.Application.Estimations.Commands.UpdateEstimation;

/// <summary>
/// Command pour mettre à jour une estimation
/// </summary>
public class UpdateEstimationCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public decimal BudgetCagnotte { get; set; }
    public decimal BudgetConsomme { get; set; }
    public Guid ModifiePar { get; set; }
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
    public decimal ordre { get; set; }
}