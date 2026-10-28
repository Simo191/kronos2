using MediatR;
using GestionPrestataires.Domain.Repositories;

namespace GestionPrestataires.Application.Cagnottes.Commands.RecalculerBudget;

public class RecalculerBudgetCommandHandler : IRequestHandler<RecalculerBudgetCommand, Unit>
{
    private readonly ICagnotteRepository _repository;

    public RecalculerBudgetCommandHandler(ICagnotteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(RecalculerBudgetCommand request, CancellationToken cancellationToken)
    {
        var cagnotte = await _repository.GetByAnneeAsync(request.Annee);
        if (cagnotte != null)
        {
            var budgetConsomme = await _repository.CalculerBudgetConsommeAsync(request.Annee);
            cagnotte.BudgetConsomme = budgetConsomme;
            await _repository.UpdateAsync(cagnotte);
        }
        
        return Unit.Value;
    }
}
