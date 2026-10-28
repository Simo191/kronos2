using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Cagnottes.Commands.CreateCagnotte;

public class CreateCagnotteCommandHandler : IRequestHandler<CreateCagnotteCommand, Cagnotte>
{
    private readonly ICagnotteRepository _repository;

    public CreateCagnotteCommandHandler(ICagnotteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Cagnotte> Handle(CreateCagnotteCommand request, CancellationToken cancellationToken)
    {
        // Vérifier si une cagnotte existe déjà pour cette année
        var existing = await _repository.GetByAnneeAsync(request.Annee);
        if (existing != null)
            throw new Exception($"Une cagnotte existe déjà pour l'année {request.Annee}");

        // Calculer le budget consommé pour cette année
        var budgetConsomme = await _repository.CalculerBudgetConsommeAsync(request.Annee);

        // Créer la cagnotte (le BudgetAnnuel sera calculé via les sources)
        var cagnotte = new Cagnotte
        {
            Id = Guid.NewGuid(),
            Annee = request.Annee,
            BudgetConsomme = budgetConsomme,
            DateCreation = DateTime.UtcNow,
            DateModification = DateTime.UtcNow,
            SourcesBudget = new List<SourceBudget>()
        };

        try
        {
            await _repository.AddAsync(cagnotte);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erreur lors de la création de la cagnotte : {ex.Message}", ex);
        }

        return cagnotte;
    }
}