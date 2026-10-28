using MediatR;

namespace GestionPrestataires.Application.Cagnottes.Commands.DeleteCagnotte;
public record DeleteCagnotteCommand(Guid Id) : IRequest<Unit>;