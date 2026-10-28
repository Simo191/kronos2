using MediatR;
using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Application.Roles.Queries.GetRoleById;

public record GetRoleByIdQuery(Guid Id) : IRequest<Role?>;
