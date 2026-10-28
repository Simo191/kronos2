using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.JoursTravail.Queries.GetAllJoursTravail;

public class GetAllJoursTravailQueryHandler : IRequestHandler<GetAllJoursTravailQuery, IEnumerable<object>>
{
    private readonly IJourTravailRepository _repository;

    public GetAllJoursTravailQueryHandler(IJourTravailRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<object>> Handle(GetAllJoursTravailQuery request, CancellationToken cancellationToken)
    {
        //   Récupérer les jours travail (Prestataire déjà Include dans le repository)
        var joursTravail = await _repository.GetAllAsync(request.PrestataireId, request.Mois);

        //   FILTRAGE : Si User, filtrer par son prestataireId
        if (request.UserRole == "User" && !string.IsNullOrEmpty(request.UserPrestataireId))
        {
            var userPrestataireId = Guid.Parse(request.UserPrestataireId);
            joursTravail = joursTravail.Where(jt => jt.PrestataireId == userPrestataireId);
        }

        var joursTravailList = joursTravail.ToList();

        //   MAPPING SELON RÔLE
        if (request.UserRole == "User")
        {
            //   USER : DTO RESTREINT (sans Tjm, sans Revenu)
            return joursTravailList.Select(jt => MapToDtoRestricted(jt)).ToList();
        }
        else
        {
            //   ADMIN/MANAGER : DTO COMPLET (avec Tjm, avec Revenu)
            return joursTravailList.Select(jt => MapToDto(jt)).ToList();
        }
    }

    //   Mapping COMPLET pour Admin/Manager (avec Tjm et Revenu)
    private static JourTravailDto MapToDto(Domain.Entities.JourTravail jourTravail)
    {
        return new JourTravailDto
        {
            Id = jourTravail.Id,
            PrestataireId = jourTravail.PrestataireId,
            Mois = jourTravail.Mois,
            NombreJours = jourTravail.NombreJours,
            Tjm = jourTravail.Tjm,  //   VISIBLE pour Admin/Manager
            Revenu = jourTravail.NombreJours * jourTravail.Tjm,  //   CALCULÉ
            Description = jourTravail.Description,
            DateTravail = jourTravail.DateTravail,
            CreatedAt = jourTravail.CreatedAt,

            // Mapper Prestataire si chargé
            Prestataire = jourTravail.Prestataire != null ? new PrestataireDtoSimple
            {
                Id = jourTravail.Prestataire.Id,
                Nom = jourTravail.Prestataire.Nom,
                Prenom = jourTravail.Prestataire.Prenom,
                Email = jourTravail.Prestataire.Email,
                ImageUrl = jourTravail.Prestataire.ImageUrl,
                //   Entreprise (utilisée pour afficher (CODE) à côté du nom dans les cartes)
                EntrepriseId = jourTravail.Prestataire.EntrepriseId,
                Entreprise = jourTravail.Prestataire.Entreprise == null ? null : new PrestataireEntrepriseDto
                {
                    Id = jourTravail.Prestataire.Entreprise.Id,
                    Nom = jourTravail.Prestataire.Entreprise.Nom,
                    Code = jourTravail.Prestataire.Entreprise.Code
                }
            } : null
        };
    }

    //   Mapping RESTREINT pour User (sans Tjm, sans Revenu)
    private static JourTravailDtoRestricted MapToDtoRestricted(Domain.Entities.JourTravail jourTravail)
    {
        return new JourTravailDtoRestricted
        {
            Id = jourTravail.Id,
            PrestataireId = jourTravail.PrestataireId,
            Mois = jourTravail.Mois,
            NombreJours = jourTravail.NombreJours,
            // ❌ Tjm = MASQUÉ
            // ❌ Revenu = MASQUÉ
            Description = jourTravail.Description,
            DateTravail = jourTravail.DateTravail,
            CreatedAt = jourTravail.CreatedAt,

            // Mapper Prestataire RESTREINT
            Prestataire = jourTravail.Prestataire != null ? new PrestataireDtoSimple
            {
                Id = jourTravail.Prestataire.Id,
                Nom = jourTravail.Prestataire.Nom,
                Prenom = jourTravail.Prestataire.Prenom,
                Email = jourTravail.Prestataire.Email,
                ImageUrl = jourTravail.Prestataire.ImageUrl,
                //   Entreprise (utilisée pour afficher (CODE) à côté du nom dans les cartes)
                EntrepriseId = jourTravail.Prestataire.EntrepriseId,
                Entreprise = jourTravail.Prestataire.Entreprise == null ? null : new PrestataireEntrepriseDto
                {
                    Id = jourTravail.Prestataire.Entreprise.Id,
                    Nom = jourTravail.Prestataire.Entreprise.Nom,
                    Code = jourTravail.Prestataire.Entreprise.Code
                }
            } : null
        };
    }
}