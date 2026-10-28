using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Cagnottes.Queries.GetCagnotteByAnnee;

public class GetCagnotteByAnneeQueryHandler : IRequestHandler<GetCagnotteByAnneeQuery, Cagnotte?>
{
    private readonly ICagnotteRepository _repository;

    public GetCagnotteByAnneeQueryHandler(ICagnotteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Cagnotte?> Handle(GetCagnotteByAnneeQuery request, CancellationToken cancellationToken)
    {
        var cagnotte = await _repository.GetByAnneeAsync(request.Annee);

        if (cagnotte != null)
        {
            var budgetConsomme = await _repository.CalculerBudgetConsommeAsync(request.Annee);
            cagnotte.BudgetConsomme = budgetConsomme;
        }

        return cagnotte;
    }
}
