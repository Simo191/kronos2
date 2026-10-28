using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Estimations.Queries.GetAllEstimations;

/// <summary>
/// Handler pour GetAllEstimationsQuery
/// </summary>
public class GetAllEstimationsQueryHandler : IRequestHandler<GetAllEstimationsQuery, List<EstimationDto>>
{
    private readonly IEstimationRepository _repository;

    public GetAllEstimationsQueryHandler(IEstimationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EstimationDto>> Handle(GetAllEstimationsQuery request, CancellationToken cancellationToken)
    {
        var estimations = await _repository.GetAllAsync();

        return estimations.Select(e => new EstimationDto
        {
            Id = e.Id,
            Annee = e.Annee,
            BudgetCagnotte = e.BudgetCagnotte,
            BudgetConsomme = e.BudgetConsomme,
            CreePar = e.CreePar,
            CreateurNom = e.Createur != null ? $"{e.Createur.FirstName} {e.Createur.LastName}" : "Inconnu",
            ModifiePar = e.ModifiePar,
            ModificateurNom = e.Modificateur != null ? $"{e.Modificateur.FirstName} {e.Modificateur.LastName}" : null,
            DateCreation = e.DateCreation,
            DateModification = e.DateModification,
            Lignes = e.Lignes.Select(l => new LigneEstimationDto
            {
                Id = l.Id,
                EstimationId = l.EstimationId,
                PrestataireId = l.PrestataireId,
                NomFuturPrestataire = l.NomFuturPrestataire,
                Mois = l.Mois,
                NombreJours = l.NombreJours,
                Tjm = l.Tjm,
                Revenu = l.Revenu,
                ordre = l.ordre,
                NomPrestataire = l.Prestataire != null
                    ? $"{l.Prestataire.Nom} {l.Prestataire.Prenom}"
                    : l.NomFuturPrestataire ?? "Inconnu"
            }).ToList()
        }).ToList();
    }
}