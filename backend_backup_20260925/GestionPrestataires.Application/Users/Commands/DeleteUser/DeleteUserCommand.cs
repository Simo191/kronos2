using MediatR;

namespace GestionPrestataires.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;
