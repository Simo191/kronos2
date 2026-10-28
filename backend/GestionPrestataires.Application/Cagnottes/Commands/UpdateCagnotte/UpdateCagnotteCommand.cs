using MediatR;

namespace GestionPrestataires.Application.Cagnottes.Commands.UpdateCagnotte;

/// <summary>
/// Commande pour mettre à jour une cagnotte
/// Note: Le BudgetAnnuel n'est plus modifiable directement, 
/// il est calculé automatiquement à partir des sources de budget
/// </summary>
public record UpdateCagnotteCommand(Guid Id) : IRequest<Unit>;