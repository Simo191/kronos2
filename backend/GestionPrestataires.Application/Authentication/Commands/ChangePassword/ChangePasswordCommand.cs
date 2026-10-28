using MediatR;

namespace GestionPrestataires.Application.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest;
