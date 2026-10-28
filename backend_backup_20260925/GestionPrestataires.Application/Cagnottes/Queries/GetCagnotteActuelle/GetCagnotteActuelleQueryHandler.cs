using MediatR;
using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;

namespace GestionPrestataires.Application.Cagnottes.Queries.GetCagnotteActuelle;

public class GetCagnotteActuelleQueryHandler : IRequestHandler<GetCagnotteActuelleQuery, Cagnotte?>
{
    private readonly ICagnotteRepository _repository;

    public GetCagnotteActuelleQueryHandler(ICagnotteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Cagnotte?> Handle(GetCagnotteActuelleQuery request, CancellationToken cancellationToken)
    {
        var anneeActuelle = DateTime.Now.Year;
        var cagnotte = await _repository.GetByAnneeAsync(anneeActuelle);
        
        if (cagnotte != null)
        {
            var budgetConsomme = await _repository.CalculerBudgetConsommeAsync(anneeActuelle);
            cagnotte.BudgetConsomme = budgetConsomme;
            await _repository.UpdateAsync(cagnotte);
        }
        
        return cagnotte;
    }
}
