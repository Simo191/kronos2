using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Prestataires.Commands.ToggleActivePrestataire;

public class ToggleActivePrestataireCommandHandler : IRequestHandler<ToggleActivePrestataireCommand, bool>
{
    private readonly IPrestataireRepository _prestataireRepository;
    private readonly IUserRepository _userRepository;

    public ToggleActivePrestataireCommandHandler(
        IPrestataireRepository prestataireRepository,
        IUserRepository userRepository)
    {
        _prestataireRepository = prestataireRepository;
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(ToggleActivePrestataireCommand request, CancellationToken cancellationToken)
    {
        var prestataire = await _prestataireRepository.GetByIdAsync(request.Id);
        if (prestataire == null)
            throw new Exception("Prestataire introuvable");

        prestataire.IsActive = !prestataire.IsActive;
        prestataire.UpdatedAt = DateTime.UtcNow;

        if (!prestataire.IsActive)
        {
            //   Désactivation : on enregistre le motif et l'horodatage
            //   (trim + tronque à 500 pour rester aligné avec HasMaxLength)
            var reason = request.Reason?.Trim();
            prestataire.DeactivationReason = string.IsNullOrEmpty(reason)
                ? null
                : (reason.Length > 500 ? reason.Substring(0, 500) : reason);
            prestataire.DeactivatedAt = DateTime.UtcNow;
        }
        else
        {
            //   Réactivation : on nettoie le motif précédent
            prestataire.DeactivationReason = null;
            prestataire.DeactivatedAt = null;
        }

        await _prestataireRepository.UpdateAsync(prestataire);

        //   Synchroniser l'état du User lié pour bloquer/débloquer la connexion
        //   (LoginCommandHandler refuse déjà les comptes avec IsActive=false)
        if (prestataire.UserId.HasValue)
        {
            var user = await _userRepository.GetByIdAsync(prestataire.UserId.Value);
            if (user != null && user.IsActive != prestataire.IsActive)
            {
                user.IsActive = prestataire.IsActive;
                await _userRepository.UpdateAsync(user);
            }
        }

        return prestataire.IsActive;
    }
}
