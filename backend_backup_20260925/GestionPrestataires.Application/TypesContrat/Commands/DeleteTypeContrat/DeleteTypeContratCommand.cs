using MediatR;

namespace GestionPrestataires.Application.TypesContrat.Commands.DeleteTypeContrat;

public record DeleteTypeContratCommand(Guid Id) : IRequest<Unit>;
