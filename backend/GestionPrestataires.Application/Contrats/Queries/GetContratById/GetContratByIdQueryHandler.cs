using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Contrats.Queries.GetContratById;

//   MODIFIÉ : Nom de classe corrigé + retourne ContratDto
public class GetContratByIdQueryHandler : IRequestHandler<GetContratByIdQuery, ContratDto?>
{
    private readonly IContratRepository _repository;
    private readonly IJourTravailRepository _context; //   ADAPTER le nom selon ton DbContext

    public GetContratByIdQueryHandler(
        IContratRepository repository,
        IJourTravailRepository context) //   AJOUTER le DbContext
    {
        _repository = repository;
        _context = context;
    }

    public async Task<ContratDto?> Handle(GetContratByIdQuery request, CancellationToken cancellationToken)
    {
        // Récupérer le contrat avec ses relations
        var contrat = await _repository.GetByIdAsync(request.Id);

        if (contrat == null)
            return null;

        //   CALCUL DES JOURS UTILISÉS : 
        // Pas de relation directe JourTravail → Contrat
        // Relation : Contrat → Prestataire ← JourTravail
        // Filtrer par PrestataireId + période du contrat
        var joursUtilises = await _context.GetbypresttaireAndContratAllAsync(contrat.PrestataireId, contrat.DateDebut, contrat.DateFin);


        //   Mapper l'entité vers le DTO
        return MapToDto(contrat, joursUtilises);
    }

    //   Méthode de mapping
    private static ContratDto MapToDto(Domain.Entities.Contrat contrat, decimal joursUtilises)
    {
        return new ContratDto
        {
            Id = contrat.Id,
            PrestataireId = contrat.PrestataireId,
            TypeContratId = contrat.TypeContratId,
            DateDebut = contrat.DateDebut,
            DateFin = contrat.DateFin,
            NombreJours = contrat.NombreJours,
            Tjm = contrat.Tjm,
            MontantTotal = contrat.MontantTotal,
            JoursUtilises = (int)Math.Round(joursUtilises), //   SOMME des NombreJours (arrondi)
            CreatedAt = contrat.CreatedAt,
            UpdatedAt = contrat.UpdatedAt,
            SeuilJoursMois = contrat.SeuilJoursMois,
            DocumentUrl = contrat.DocumentUrl!,
            Active = contrat.Active,
            NumeroCommande = contrat.NumeroCommande,
            ReferenceCtr = contrat.ReferenceCtr,
            Tranche = contrat.Tranche,
            ApplicationId = contrat.ApplicationId,
            Commentaire = contrat.Commentaire,

            // Mapper les relations si elles sont chargées
            Prestataire = contrat.Prestataire != null ? new PrestataireDto
            {
                Id = contrat.Prestataire.Id,
                Nom = contrat.Prestataire.Nom,
                Prenom = contrat.Prestataire.Prenom,
                Email = contrat.Prestataire.Email,
                Telephone = contrat.Prestataire.Telephone,
                Tjm = contrat.Prestataire.Tjm
            } : null,

            TypeContrat = contrat.TypeContrat != null ? new TypeContratDto
            {
                Id = contrat.TypeContrat.Id,
                Libelle = contrat.TypeContrat.Libelle,
                Description = contrat.TypeContrat.Description
            } : null
        };
    }
}