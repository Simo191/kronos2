using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Roles.Commands.CreateRole;

public record CreateRoleCommand(string Nom, string? Description) : IRequest<Role>;
