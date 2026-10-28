using MediatR;
using GestionPrestataires.Domain.Repositories;

namespace GestionPrestataires.Application.Cagnottes.Commands.DeleteSourceBudget;

public class DeleteSourceBudgetCommandHandler : IRequestHandler<DeleteSourceBudgetCommand, Unit>
{
    private readonly ISourceBudgetRepository _sourceBudgetRepository;
    private readonly ICagnotteRepository _cagnotteRepository;

    public DeleteSourceBudgetCommandHandler(
        ISourceBudgetRepository sourceBudgetRepository,
        ICagnotteRepository cagnotteRepository)
    {
        _sourceBudgetRepository = sourceBudgetRepository;
        _cagnotteRepository = cagnotteRepository;
    }

    public async Task<Unit> Handle(DeleteSourceBudgetCommand request, CancellationToken cancellationToken)
    {
        var sourceBudget = await _sourceBudgetRepository.GetByIdAsync(request.Id);
        if (sourceBudget == null)
            throw new Exception("Source de budget non trouvée");

        var cagnotteId = sourceBudget.CagnotteId;

        await _sourceBudgetRepository.DeleteAsync(request.Id);

        // Mise à jour de la date de modification de la cagnotte
        var cagnotte = await _cagnotteRepository.GetByIdAsync(cagnotteId);
        if (cagnotte != null)
        {
            cagnotte.DateModification = DateTime.UtcNow;
            await _cagnotteRepository.UpdateAsync(cagnotte);
        }

        return Unit.Value;
    }
}