using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Estimations.Commands.CreateEstimation;

// <summary>
/// Handler pour CreateEstimationCommand
/// </summary>
public class CreateEstimationCommandHandler : IRequestHandler<CreateEstimationCommand, Guid>
{
    private readonly IEstimationRepository _repository;

    public CreateEstimationCommandHandler(IEstimationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateEstimationCommand request, CancellationToken cancellationToken)
    {
        // Vérifier qu'il n'existe pas déjà une estimation pour cette année
        var existante = await _repository.GetByAnneeAsync(request.Annee);
        if (existante != null)
            throw new InvalidOperationException($"Une estimation existe déjà pour l'année {request.Annee}");

        var estimation = new Estimation
        {
            Id = Guid.NewGuid(),
            Annee = request.Annee,
            BudgetCagnotte = request.BudgetCagnotte,
            BudgetConsomme = request.BudgetConsomme,
            CreePar = request.CreePar,
            DateCreation = DateTime.UtcNow,
            DateModification = DateTime.UtcNow,
            Lignes = request.Lignes.Select(l => new LigneEstimation
            {
                Id = Guid.NewGuid(),
                PrestataireId = l.PrestataireId,
                NomFuturPrestataire = l.NomFuturPrestataire,
                Mois = l.Mois,
                NombreJours = l.NombreJours,
                Tjm = l.Tjm,
                ordre = l.ordre,
                Revenu = l.NombreJours * l.Tjm
            }).ToList()
        };

        await _repository.AddAsync(estimation);
        return estimation.Id;
    }
}