using MediatR;
using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;

namespace GestionPrestataires.Application.Cagnottes.Queries.GetAllCagnottes;

public class GetAllCagnottesQueryHandler : IRequestHandler<GetAllCagnottesQuery, List<Cagnotte>>
{
    private readonly ICagnotteRepository _repository;

    public GetAllCagnottesQueryHandler(ICagnotteRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Cagnotte>> Handle(GetAllCagnottesQuery request, CancellationToken cancellationToken)
    {
        var cagnottes = await _repository.GetAllAsync();
        
        foreach (var cagnotte in cagnottes)
        {
            var budgetConsomme = await _repository.CalculerBudgetConsommeAsync(cagnotte.Annee);
            cagnotte.BudgetConsomme = budgetConsomme;
        }
        
        return cagnottes;
    }
}
