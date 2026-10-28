using MediatR;

namespace GestionPrestataires.Application.Cagnottes.Commands.DeleteSourceBudget;

public record DeleteSourceBudgetCommand(Guid Id) : IRequest<Unit>;