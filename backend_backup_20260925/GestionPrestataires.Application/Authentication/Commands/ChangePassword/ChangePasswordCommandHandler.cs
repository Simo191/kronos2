using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _repository;

    public ChangePasswordCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.UserId);

        if (user == null)
            throw new UnauthorizedAccessException("Utilisateur non trouvé");

        // Vérifier que le mot de passe actuel est correct
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Mot de passe actuel incorrect");

        // Hasher le nouveau mot de passe
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        //user. = DateTime.UtcNow;

        await _repository.UpdateAsync(user);
    }
}