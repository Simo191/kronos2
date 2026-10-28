using MediatR;

namespace GestionPrestataires.Application.Estimations.Commands.CreateEstimation;

public class CreateEstimationCommand : IRequest<Guid>
{
    public int Annee { get; set; }
    public decimal BudgetCagnotte { get; set; }
    public decimal BudgetConsomme { get; set; }
    public Guid CreePar { get; set; }
    public List<CreateLigneEstimationDto> Lignes { get; set; } = new();
}

public class CreateLigneEstimationDto
{
    public Guid? PrestataireId { get; set; }
    public string? NomFuturPrestataire { get; set; }
    public string Mois { get; set; } = string.Empty;
    public decimal NombreJours { get; set; }
    public decimal Tjm { get; set; }
    public decimal ordre { get; set; }
}