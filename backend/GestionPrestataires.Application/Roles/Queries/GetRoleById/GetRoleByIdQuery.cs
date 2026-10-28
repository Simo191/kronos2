using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Roles.Queries.GetRoleById;

public record GetRoleByIdQuery(Guid Id) : IRequest<Role?>;
