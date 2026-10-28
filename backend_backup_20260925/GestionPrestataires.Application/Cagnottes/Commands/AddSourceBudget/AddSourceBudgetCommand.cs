using MediatR;
using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Application.Cagnottes.Commands.AddSourceBudget;

public record AddSourceBudgetCommand(
    Guid CagnotteId,
    string Nom,
    string Code,
    decimal Montant,
    string? Description
) : IRequest<SourceBudget>;