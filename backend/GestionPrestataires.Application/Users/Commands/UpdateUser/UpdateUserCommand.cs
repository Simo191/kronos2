using MediatR;

namespace GestionPrestataires.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    Guid RoleId,
    bool IsActive,          //   ACTIVATION/DÉSACTIVATION
    Guid? PrestataireId = null //   Lier/délier prestataire
) : IRequest;