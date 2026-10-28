using MediatR;

namespace GestionPrestataires.Application.TypesContrat.Commands.UpdateTypeContrat;

public record UpdateTypeContratCommand(Guid Id, string Libelle, string? Description) : IRequest<Unit>;
