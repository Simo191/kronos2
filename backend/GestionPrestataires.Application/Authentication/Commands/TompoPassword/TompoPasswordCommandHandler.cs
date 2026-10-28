using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Authentication.Commands.TompoPassword
{

    public class TompoPasswordCommandHandler : IRequestHandler<TompoPasswordCommand>
    {
        private readonly IUserRepository _repository;

        public TompoPasswordCommandHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(TompoPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.UserId);

            if (user == null)
                throw new UnauthorizedAccessException("Utilisateur non trouvé");

            // Hasher le nouveau mot de passe
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            //user. = DateTime.UtcNow;

            await _repository.UpdateAsync(user);
        }
    }
}
