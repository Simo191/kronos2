using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.JoursTravail.Commands.CreateJourTravail;

public class CreateJourTravailCommandHandler : IRequestHandler<CreateJourTravailCommand, Guid>
{
    private readonly IJourTravailRepository _repository;
    private readonly IMoisVerrouillRepository _moisVerrouillRepository;


    public CreateJourTravailCommandHandler(IJourTravailRepository repository, IMoisVerrouillRepository moisVerrouillRepository)
    {
        _repository = repository;
        _moisVerrouillRepository = moisVerrouillRepository;

    }

    public async Task<Guid> Handle(CreateJourTravailCommand request, CancellationToken cancellationToken)
    {
        //   Vérifier si le mois est verrouillé
        var estVerrouille = await _moisVerrouillRepository.IsMoisVerrouille(request.Mois);
        if (estVerrouille)
            throw new Exception($"Le mois {request.Mois} est verrouillé. Impossible de créer des jours de travail.");


        var datejour = Convert.ToDateTime(request.Mois);
        var dateTravail = new DateTime(datejour.Year, datejour.Month, 01, 00, 00, 59, DateTimeKind.Utc);
        var jourTravail = new JourTravail
        {
            Id = Guid.NewGuid(),
            PrestataireId = request.PrestataireId,
            Mois = request.Mois,
            Tjm = request.Tjm,
            NombreJours = request.NombreJours,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            DateTravail = dateTravail
        };

        await _repository.AddAsync(jourTravail);
        return jourTravail.Id;
    }
}
