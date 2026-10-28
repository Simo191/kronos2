using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.JoursTravail.Commands.DeleteJourTravail;

public class DeleteJourTravailCommandHandler : IRequestHandler<DeleteJourTravailCommand>
{
    private readonly IJourTravailRepository _repository;
    private readonly IMoisVerrouillRepository _moisVerrouillRepository;

    public DeleteJourTravailCommandHandler(IJourTravailRepository repository, IMoisVerrouillRepository moisVerrouillRepository)
    {
        _repository = repository;
        _moisVerrouillRepository = moisVerrouillRepository;

    }

    public async Task Handle(DeleteJourTravailCommand request, CancellationToken cancellationToken)
    {
        //  Récupérer le jour de travail pour connaître son mois
        var jourTravail = await _repository.GetByIdAsync(request.Id);
        if (jourTravail == null)
            throw new Exception("Jour de travail introuvable");

        // Vérifier si le mois est verrouillé
        var estVerrouille = await _moisVerrouillRepository.IsMoisVerrouille(jourTravail.Mois);
        if (estVerrouille)
            throw new Exception($"Le mois {jourTravail.Mois} est verrouillé. Impossible de supprimer des jours de travail.");

        await _repository.DeleteAsync(request.Id);
    }
}
