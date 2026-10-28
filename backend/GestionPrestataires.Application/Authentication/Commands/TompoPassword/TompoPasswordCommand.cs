using MediatR;

namespace GestionPrestataires.Application.Authentication.Commands.TompoPassword;
public record TompoPasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest;