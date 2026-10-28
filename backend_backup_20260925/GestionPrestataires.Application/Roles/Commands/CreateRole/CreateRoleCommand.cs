using MediatR;
using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Application.Roles.Commands.CreateRole;

public record CreateRoleCommand(string Nom, string? Description) : IRequest<Role>;
