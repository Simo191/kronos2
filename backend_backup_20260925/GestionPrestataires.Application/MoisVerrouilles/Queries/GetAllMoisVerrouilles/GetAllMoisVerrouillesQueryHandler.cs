using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.MoisVerrouilles.Queries.GetAllMoisVerrouilles;

public class GetAllMoisVerrouillesQueryHandler : IRequestHandler<GetAllMoisVerrouillesQuery, IEnumerable<MoisVerrouillDto>>
{
    private readonly IMoisVerrouillRepository _repository;

    public GetAllMoisVerrouillesQueryHandler(IMoisVerrouillRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<MoisVerrouillDto>> Handle(GetAllMoisVerrouillesQuery request, CancellationToken cancellationToken)
    {
        var moisVerrouilles = await _repository.GetAllAsync();

        return moisVerrouilles.Select(m => new MoisVerrouillDto
        {
            Id = m.Id,
            Mois = m.Mois,
            VerrouilleParUserId = m.VerrouilleParUserId,
            VerrouilleParUserName = m.VerrouilleParUser != null
                ? $"{m.VerrouilleParUser.FirstName} {m.VerrouilleParUser.LastName}"
                : "Inconnu",
            DateVerrouillage = m.DateVerrouillage,
            Commentaire = m.Commentaire
        }).ToList();
    }
}