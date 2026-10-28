using MediatR;

namespace GestionPrestataires.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    Guid RoleId,
    Guid? PrestataireId = null //   Lier à un prestataire (optionnel)
) : IRequest<Guid>;