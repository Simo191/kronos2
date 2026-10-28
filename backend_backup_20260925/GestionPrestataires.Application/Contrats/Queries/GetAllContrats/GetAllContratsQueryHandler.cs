using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Application.Contrats.Queries.GetAllContrats;

public class GetAllContratsQueryHandler : IRequestHandler<GetAllContratsQuery, IEnumerable<object>>
{
    private readonly IContratRepository _repository;
    private readonly IJourTravailRepository _context;

    public GetAllContratsQueryHandler(
        IContratRepository repository,
        IJourTravailRepository context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<IEnumerable<object>> Handle(GetAllContratsQuery request, CancellationToken cancellationToken)
    {
        // Récupérer tous les contrats
        var contratsQuery = _repository.GetAllAsync();

        //   FILTRAGE: Si User, filtrer par prestataireId
        if (request.UserRole == "User" && !string.IsNullOrEmpty(request.UserPrestataireId))
        {
            var prestataireId = Guid.Parse(request.UserPrestataireId);
            contratsQuery = contratsQuery.Where(c => c.PrestataireId == prestataireId);
        }
        var contrats = await contratsQuery.ToListAsync(cancellationToken);

        //   Charger tous les JoursTravail en mémoire pour calcul
        var joursTravail = await _context.GetAllAsync();

        //   MAPPING SELON RÔLE
        if (request.UserRole == "User")
        {
            //   USER : DTO RESTREINT (sans TJM, sans MontantTotal)
            return contrats.Select(contrat =>
            {
                // Calculer les jours utilisés
                var joursUtilises = joursTravail
                    .Where(jt => jt.PrestataireId == contrat.PrestataireId
                              && jt.DateTravail >= contrat.DateDebut
                              && jt.DateTravail <= contrat.DateFin)
                    .Sum(jt => jt.NombreJours);

                return MapToDtoRestricted(contrat, joursUtilises);
            }).ToList();
        }
        else
        {
            //   ADMIN/MANAGER : DTO COMPLET (avec TJM, avec MontantTotal)
            return contrats.Select(contrat =>
            {
                // Calculer les jours utilisés
                var joursUtilises = joursTravail
                    .Where(jt => jt.PrestataireId == contrat.PrestataireId
                              && jt.DateTravail >= contrat.DateDebut
                              && jt.DateTravail <= contrat.DateFin)
                    .Sum(jt => jt.NombreJours);

                return MapToDto(contrat, joursUtilises);
            }).ToList();
        }
    }

    //   Mapping COMPLET pour Admin/Manager (avec TJM)
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
            Tjm = contrat.Tjm,  //   VISIBLE pour Admin/Manager
            MontantTotal = contrat.MontantTotal,  //   VISIBLE pour Admin/Manager
            JoursUtilises = (int)Math.Round(joursUtilises),
            CreatedAt = contrat.CreatedAt,
            UpdatedAt = contrat.UpdatedAt,
            SeuilJoursMois = contrat.SeuilJoursMois,
            DocumentUrl = contrat.DocumentUrl!,
            Active = contrat.Active,
            // Mapper les relations si chargées
            Prestataire = contrat.Prestataire != null ? new PrestataireDto
            {
                Id = contrat.Prestataire.Id,
                Nom = contrat.Prestataire.Nom,
                Prenom = contrat.Prestataire.Prenom,
                Email = contrat.Prestataire.Email,
                Telephone = contrat.Prestataire.Telephone,
                Tjm = contrat.Prestataire.Tjm,  //   VISIBLE pour Admin/Manager
                ImageUrl = contrat.Prestataire.ImageUrl
            } : null,

            TypeContrat = contrat.TypeContrat != null ? new TypeContratDto
            {
                Id = contrat.TypeContrat.Id,
                Libelle = contrat.TypeContrat.Libelle,
                Description = contrat.TypeContrat.Description
            } : null
        };
    }

    //   Mapping RESTREINT pour User (sans TJM, sans MontantTotal)
    private static ContratDtoRestricted MapToDtoRestricted(Domain.Entities.Contrat contrat, decimal joursUtilises)
    {
        return new ContratDtoRestricted
        {
            Id = contrat.Id,
            PrestataireId = contrat.PrestataireId,
            TypeContratId = contrat.TypeContratId,
            DateDebut = contrat.DateDebut,
            DateFin = contrat.DateFin,
            NombreJours = contrat.NombreJours,
            // ❌ Tjm = MASQUÉ
            // ❌ MontantTotal = MASQUÉ
            JoursUtilises = joursUtilises,
            CreatedAt = contrat.CreatedAt,
            UpdatedAt = contrat.UpdatedAt,
            SeuilJoursMois=contrat.SeuilJoursMois,
            Active = contrat.Active,

            // Mapper les relations RESTREINTES
            Prestataire = contrat.Prestataire != null ? new PrestataireDtoRestricted
            {
                Id = contrat.Prestataire.Id,
                Nom = contrat.Prestataire.Nom,
                Prenom = contrat.Prestataire.Prenom,
                Email = contrat.Prestataire.Email,
                Telephone = contrat.Prestataire.Telephone,
                ImageUrl = contrat.Prestataire.ImageUrl
                // ❌ Tjm = MASQUÉ
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