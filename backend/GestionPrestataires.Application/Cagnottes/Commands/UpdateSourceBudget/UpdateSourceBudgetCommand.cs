using MediatR;

namespace GestionPrestataires.Application.Cagnottes.Commands.UpdateSourceBudget;

public record UpdateSourceBudgetCommand(
    Guid Id,
    string Nom,
    string Code,
    decimal Montant,
    string? Description
) : IRequest<Unit>;