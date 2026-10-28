using MediatR;

namespace GestionPrestataires.Application.Roles.Commands.DeleteRole;

public record DeleteRoleCommand(Guid Id) : IRequest<Unit>;
