using MediatR;
using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Application.Roles.Queries.GetAllRoles;

public record GetAllRolesQuery : IRequest<List<Role>>;
