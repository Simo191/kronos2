using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.Estimations.Queries.GetEstimationByAnnee;
/// <summary>
/// Handler pour GetEstimationByAnneeQuery
/// </summary>
public class GetEstimationByAnneeQueryHandler : IRequestHandler<GetEstimationByAnneeQuery, EstimationDto?>
{
    private readonly IEstimationRepository _repository;

    public GetEstimationByAnneeQueryHandler(IEstimationRepository repository)
    {
        _repository = repository;
    }

    public async Task<EstimationDto?> Handle(GetEstimationByAnneeQuery request, CancellationToken cancellationToken)
    {
        var estimation = await _repository.GetByAnneeAsync(request.Annee);
        if (estimation == null)
            return null;

        return new EstimationDto
        {
            Id = estimation.Id,
            Annee = estimation.Annee,
            BudgetCagnotte = estimation.BudgetCagnotte,
            BudgetConsomme = estimation.BudgetConsomme,
            CreePar = estimation.CreePar,
            CreateurNom = estimation.Createur != null ? $"{estimation.Createur.FirstName} {estimation.Createur.LastName}" : "Inconnu",
            DateCreation = estimation.DateCreation,
            DateModification = estimation.DateModification,
            Lignes = estimation.Lignes.Select(l => new LigneEstimationDto
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
        };
    }
}