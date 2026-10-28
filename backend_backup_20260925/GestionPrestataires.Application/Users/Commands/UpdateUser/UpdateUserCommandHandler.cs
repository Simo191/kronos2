using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository _repository;
    private readonly IPrestataireRepository _context;

    public UpdateUserCommandHandler(IUserRepository repository, IPrestataireRepository context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id);
        if (user == null)
            throw new Exception("Utilisateur non trouvé");

        //   Vérifier email unique (sauf pour lui-même)
        var existingUser = await _repository.GetByEmailAndIDAsync(request.Email, request.Id);

        if (existingUser != null)
            throw new Exception("Un autre utilisateur avec cet email existe déjà");

        //   GÉRER LA RELATION PRESTATAIRE
        var currentPrestataire = await _context.GetByIdUserAsync(user.Id);

        // Si nouvelle liaison demandée
        if (request.PrestataireId.HasValue)
        {
            var newPrestataire = await _context.GetByIdAsync(request.PrestataireId.Value);

            if (newPrestataire == null)
                throw new Exception("Prestataire non trouvé");

            //   Vérifier que le nouveau prestataire n'a pas déjà un user (sauf si c'est le même)
            if (newPrestataire.UserId != null && newPrestataire.UserId != user.Id)
                throw new Exception("Ce prestataire est déjà lié à un autre utilisateur");

            // Délier l'ancien prestataire si différent
            if (currentPrestataire != null && currentPrestataire.Id != request.PrestataireId.Value)
            {
                currentPrestataire.UserId = null;
                currentPrestataire.UpdatedAt = DateTime.UtcNow;
            }

            // Lier le nouveau prestataire
            if (newPrestataire.Id != currentPrestataire?.Id)
            {
                newPrestataire.UserId = user.Id;
                newPrestataire.UpdatedAt = DateTime.UtcNow;
            }
            await _context.UpdateAsync(newPrestataire!);

        }
        else // Pas de liaison demandée
        {
            // Délier le prestataire actuel
            if (currentPrestataire != null)
            {
                currentPrestataire.UserId = null;
                currentPrestataire.UpdatedAt = DateTime.UtcNow;
            }
        }

        // Mettre à jour l'utilisateur
        user.Email = request.Email;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.RoleId = request.RoleId;
        user.IsActive = request.IsActive; //   ACTIVATION/DÉSACTIVATION
        if (currentPrestataire != null)
            await _context.UpdateAsync(currentPrestataire!);
        await _repository.UpdateAsync(user);
    }
}