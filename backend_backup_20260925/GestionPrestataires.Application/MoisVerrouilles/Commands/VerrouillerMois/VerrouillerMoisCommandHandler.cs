using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.MoisVerrouilles.Commands.VerrouillerMois;

public class VerrouillerMoisCommandHandler : IRequestHandler<VerrouillerMoisCommand, Guid>
{
    private readonly IMoisVerrouillRepository _repository;

    public VerrouillerMoisCommandHandler(IMoisVerrouillRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(VerrouillerMoisCommand request, CancellationToken cancellationToken)
    {
        // Vérifier si le mois est déjà verrouillé
        var existe = await _repository.GetByMoisAsync(request.Mois);
        if (existe != null)
            throw new Exception($"Le mois {request.Mois} est déjà verrouillé");

        var moisVerrouille = new MoisVerrouille
        {
            Id = Guid.NewGuid(),
            Mois = request.Mois,
            VerrouilleParUserId = request.VerrouilleParUserId,
            DateVerrouillage = DateTime.UtcNow,
            Commentaire = request.Commentaire
        };

        await _repository.AddAsync(moisVerrouille);
        return moisVerrouille.Id;
    }
}