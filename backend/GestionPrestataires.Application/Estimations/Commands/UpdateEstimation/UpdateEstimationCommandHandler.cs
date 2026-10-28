using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Estimations.Commands.UpdateEstimation;

/// <summary>
/// Handler pour UpdateEstimationCommand
/// </summary>
public class UpdateEstimationCommandHandler : IRequestHandler<UpdateEstimationCommand, Unit>
{
    private readonly IEstimationRepository _repository;
    private readonly ILigneEstimationRepository _ligneEstimationRepository;

    public UpdateEstimationCommandHandler(IEstimationRepository repository, ILigneEstimationRepository ligneEstimationRepository)
    {
        _repository = repository;
        _ligneEstimationRepository = ligneEstimationRepository;
    }

    public async Task<Unit> Handle(UpdateEstimationCommand request, CancellationToken cancellationToken)
    {
        var estimation = await _repository.GetByIdWithLignesNotrackAsync(request.Id);
        if (estimation == null)
            throw new InvalidOperationException("Estimation introuvable");

        estimation.BudgetCagnotte = request.BudgetCagnotte;
        estimation.BudgetConsomme = request.BudgetConsomme;
        estimation.DateModification = DateTime.UtcNow;
        estimation.ModifiePar = request.ModifiePar;

        // Supprimer les anciennes lignes
        var ligne = await _ligneEstimationRepository.GetByIdWithLignesAsync(request.Id);
        if (ligne.Count != 0)
            await _ligneEstimationRepository.DeleteRangeAsync(ligne);

        // Ajouter les nouvelles lignes
        List<LigneEstimation> listligneestimations = request.Lignes
            .Select(l => new LigneEstimation
            {
                Id = l.Id ?? Guid.NewGuid(),
                EstimationId = request.Id,
                PrestataireId = l.PrestataireId,
                NomFuturPrestataire = l.NomFuturPrestataire,
                Mois = l.Mois,
                NombreJours = l.NombreJours,
                Tjm = l.Tjm,
                ordre = l.ordre,
                Revenu = l.NombreJours * l.Tjm
            })
            .ToList();


        await _repository.UpdateAsync(estimation);
        await _ligneEstimationRepository.AddRangeAsync(listligneestimations);
        return Unit.Value;
    }
}