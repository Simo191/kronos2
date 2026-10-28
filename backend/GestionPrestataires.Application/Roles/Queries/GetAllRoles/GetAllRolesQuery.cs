using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Roles.Queries.GetAllRoles;

public record GetAllRolesQuery : IRequest<List<Role>>;
