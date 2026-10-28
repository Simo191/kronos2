using MediatR;
using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;

namespace GestionPrestataires.Application.Cagnottes.Commands.AddSourceBudget;

public class AddSourceBudgetCommandHandler : IRequestHandler<AddSourceBudgetCommand, SourceBudget>
{
    private readonly ISourceBudgetRepository _sourceBudgetRepository;
    private readonly ICagnotteRepository _cagnotteRepository;

    public AddSourceBudgetCommandHandler(
        ISourceBudgetRepository sourceBudgetRepository,
        ICagnotteRepository cagnotteRepository)
    {
        _sourceBudgetRepository = sourceBudgetRepository;
        _cagnotteRepository = cagnotteRepository;
    }

    public async Task<SourceBudget> Handle(AddSourceBudgetCommand request, CancellationToken cancellationToken)
    {
        var cagnotte = await _cagnotteRepository.GetByIdAsync(request.CagnotteId);
        if (cagnotte == null)
            throw new Exception("Cagnotte non trouvée");

        var sourceBudget = new SourceBudget
        {
            Id = Guid.NewGuid(),
            CagnotteId = request.CagnotteId,
            Nom = request.Nom,
            Code = request.Code,
            Montant = request.Montant,
            Description = request.Description,
            DateCreation = DateTime.UtcNow,
            DateModification = DateTime.UtcNow
        };

        await _sourceBudgetRepository.AddAsync(sourceBudget);

        // Mise à jour de la date de modification de la cagnotte
        cagnotte.DateModification = DateTime.UtcNow;
        await _cagnotteRepository.UpdateAsync(cagnotte);

        return sourceBudget;
    }
}