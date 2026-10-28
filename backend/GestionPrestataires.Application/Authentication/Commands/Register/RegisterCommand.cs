using MediatR;

namespace GestionPrestataires.Application.Authentication.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : IRequest<Guid>;
