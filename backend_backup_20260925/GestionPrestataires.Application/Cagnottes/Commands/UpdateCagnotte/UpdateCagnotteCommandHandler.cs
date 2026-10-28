using MediatR;
using GestionPrestataires.Domain.Repositories;

namespace GestionPrestataires.Application.Cagnottes.Commands.UpdateCagnotte;

public class UpdateCagnotteCommandHandler : IRequestHandler<UpdateCagnotteCommand, Unit>
{
    private readonly ICagnotteRepository _repository;

    public UpdateCagnotteCommandHandler(ICagnotteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateCagnotteCommand request, CancellationToken cancellationToken)
    {
        var cagnotte = await _repository.GetByIdAsync(request.Id);
        if (cagnotte == null)
            throw new Exception("Cagnotte non trouvée");

        // Le BudgetAnnuel est maintenant calculé automatiquement via les sources de budget
        // On ne le met plus à jour manuellement

        // Recalculer le budget consommé
        var budgetConsomme = await _repository.CalculerBudgetConsommeAsync(cagnotte.Annee);
        cagnotte.BudgetConsomme = budgetConsomme;

        // Mettre à jour la date de modification
        cagnotte.DateModification = DateTime.UtcNow;

        await _repository.UpdateAsync(cagnotte);
        return Unit.Value;
    }
}