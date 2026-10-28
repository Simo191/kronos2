using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Cagnottes.Commands.AddSourceBudget;

public record AddSourceBudgetCommand(
    Guid CagnotteId,
    string Nom,
    string Code,
    decimal Montant,
    string? Description
) : IRequest<SourceBudget>;