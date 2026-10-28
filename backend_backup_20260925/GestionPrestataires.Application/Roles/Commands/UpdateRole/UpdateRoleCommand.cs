using MediatR;

namespace GestionPrestataires.Application.Roles.Commands.UpdateRole;

public record UpdateRoleCommand(Guid Id, string Nom, string? Description) : IRequest<Unit>;
