using MediatR;
using GestionPrestataires.Domain.Repositories;

namespace GestionPrestataires.Application.Cagnottes.Commands.UpdateSourceBudget;

public class UpdateSourceBudgetCommandHandler : IRequestHandler<UpdateSourceBudgetCommand, Unit>
{
    private readonly ISourceBudgetRepository _sourceBudgetRepository;
    private readonly ICagnotteRepository _cagnotteRepository;

    public UpdateSourceBudgetCommandHandler(
        ISourceBudgetRepository sourceBudgetRepository,
        ICagnotteRepository cagnotteRepository)
    {
        _sourceBudgetRepository = sourceBudgetRepository;
        _cagnotteRepository = cagnotteRepository;
    }

    public async Task<Unit> Handle(UpdateSourceBudgetCommand request, CancellationToken cancellationToken)
    {
        var sourceBudget = await _sourceBudgetRepository.GetByIdAsync(request.Id);
        if (sourceBudget == null)
            throw new Exception("Source de budget non trouvée");

        sourceBudget.Nom = request.Nom;
        sourceBudget.Code = request.Code;
        sourceBudget.Montant = request.Montant;
        sourceBudget.Description = request.Description;
        sourceBudget.DateModification = DateTime.UtcNow;

        await _sourceBudgetRepository.UpdateAsync(sourceBudget);

        // Mise à jour de la date de modification de la cagnotte
        var cagnotte = await _cagnotteRepository.GetByIdAsync(sourceBudget.CagnotteId);
        if (cagnotte != null)
        {
            cagnotte.DateModification = DateTime.UtcNow;
            await _cagnotteRepository.UpdateAsync(cagnotte);
        }

        return Unit.Value;
    }
}