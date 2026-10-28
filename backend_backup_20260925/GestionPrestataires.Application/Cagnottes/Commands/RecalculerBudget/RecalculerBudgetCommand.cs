using MediatR;

namespace GestionPrestataires.Application.Cagnottes.Commands.RecalculerBudget;

public record RecalculerBudgetCommand(int Annee) : IRequest<Unit>;
