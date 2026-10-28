using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Application.Contrats.Queries.GetSuiviCommandes;

//   Tableau de suivi des commandes : une ligne par contrat (tranche ferme ou option),
//   ventilée par année, avec la consommation réelle issue des jours travaillés.
public class GetSuiviCommandesQueryHandler : IRequestHandler<GetSuiviCommandesQuery, SuiviCommandesDto>
{
    private readonly IContratRepository _contratRepository;
    private readonly IJourTravailRepository _jourTravailRepository;
    private readonly IApplicationPlanningRepository _applicationRepository;

    public GetSuiviCommandesQueryHandler(
        IContratRepository contratRepository,
        IJourTravailRepository jourTravailRepository,
        IApplicationPlanningRepository applicationRepository)
    {
        _contratRepository = contratRepository;
        _jourTravailRepository = jourTravailRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<SuiviCommandesDto> Handle(GetSuiviCommandesQuery request, CancellationToken cancellationToken)
    {
        var contrats = await _contratRepository.GetAllAsync().ToListAsync(cancellationToken);
        var joursTravail = (await _jourTravailRepository.GetAllAsync()).ToList();
        var applications = (await _applicationRepository.GetAllAsync()).ToList();   // déjà triées (ordre du planning)

        //   prestataire → logiciels où il est affecté dans le planning
        var logicielsParPrestataire = applications
            .SelectMany(a => a.Assignments.Select(ass => new { ass.PrestataireId, Logiciel = new SuiviLogicielDto { Id = a.Id, Nom = a.Nom } }))
            .GroupBy(x => x.PrestataireId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Logiciel).GroupBy(l => l.Id).Select(l => l.First()).ToList());

        return new SuiviCommandesDto
        {
            Applications = applications.Select(a => new SuiviApplicationDto
            {
                Id = a.Id,
                Nom = a.Nom,
                Couleur = a.Couleur,
                Statut = a.Statut,
                Ordre = a.Ordre
            }).ToList(),
            Lignes = contrats.Select(c => MapLigne(c, joursTravail,
                logicielsParPrestataire.GetValueOrDefault(c.PrestataireId) ?? new List<SuiviLogicielDto>())).ToList()
        };
    }

    private static SuiviCommandeLigneDto MapLigne(Contrat c, List<JourTravail> joursTravail, List<SuiviLogicielDto> logicielsPlanning)
    {
        //   Logiciel : choisi dans le contrat, sinon le (premier) logiciel du planning
        var (applicationId, applicationNom, source) =
            c.ApplicationId.HasValue
                ? (c.ApplicationId, c.Application?.Nom, SourcesLogiciel.Contrat)
                : logicielsPlanning.Count > 0
                    ? ((Guid?)logicielsPlanning[0].Id, logicielsPlanning[0].Nom, SourcesLogiciel.Planning)
                    : ((Guid?)null, (string?)null, SourcesLogiciel.Aucun);

        //   Même règle que la validation du quota : un mois compte s'il chevauche la période du contrat
        var consommation = joursTravail
            .Where(jt => jt.PrestataireId == c.PrestataireId && MoisChevauche(jt, c))
            .Select(jt => new
            {
                Annee = AnneeDuMois(jt),
                Jours = jt.NombreJours,
                Montant = jt.NombreJours * (jt.Tjm > 0 ? jt.Tjm : c.Tjm)
            })
            .ToList();

        var repartitionAuto = c.Repartitions.Count == 0;
        var prevus = repartitionAuto
            ? RepartitionAnnuelleCalculator.Proposer(c.DateDebut, c.DateFin, c.NombreJours)
            : c.Repartitions.OrderBy(r => r.Annee).Select(r => (r.Annee, r.NombreJours)).ToList();

        var annees = prevus.Select(p => p.Item1)
            .Union(consommation.Select(x => x.Annee))
            .OrderBy(a => a)
            .Select(a =>
            {
                var jours = prevus.Where(p => p.Item1 == a).Sum(p => p.Item2);
                return new SuiviAnneeDto
                {
                    Annee = a,
                    JoursPrevus = jours,
                    MontantPrevu = MontantPourJours(c, jours),
                    JoursConsommes = consommation.Where(x => x.Annee == a).Sum(x => x.Jours),
                    MontantConsomme = consommation.Where(x => x.Annee == a).Sum(x => x.Montant)
                };
            })
            .ToList();

        //   Les montants annuels doivent retomber exactement sur le montant du contrat
        var avecPrevu = annees.Where(a => a.JoursPrevus > 0).ToList();
        if (avecPrevu.Count > 0 && avecPrevu.Sum(a => a.JoursPrevus) == c.NombreJours)
            avecPrevu[^1].MontantPrevu = c.MontantTotal - avecPrevu.Take(avecPrevu.Count - 1).Sum(a => a.MontantPrevu);

        var joursConsommes = consommation.Sum(x => x.Jours);
        var montantConsomme = consommation.Sum(x => x.Montant);

        return new SuiviCommandeLigneDto
        {
            ContratId = c.Id,
            ApplicationId = applicationId,
            ApplicationNom = applicationNom,
            ApplicationSource = source,
            LogicielsPlanning = logicielsPlanning,
            NumeroCommande = c.NumeroCommande,
            Tranche = c.Tranche,
            ReferenceCtr = c.ReferenceCtr,
            Commentaire = c.Commentaire,
            TypeContrat = c.TypeContrat?.Libelle,
            Active = c.Active,
            PrestataireId = c.PrestataireId,
            PrestataireNom = c.Prestataire == null ? "" : $"{c.Prestataire.Nom} {c.Prestataire.Prenom}".Trim(),
            PrestataireImageUrl = c.Prestataire?.ImageUrl,
            PrestataireActif = c.Prestataire?.IsActive ?? true,
            EntrepriseNom = c.Prestataire?.Entreprise?.Nom,
            EntrepriseCode = c.Prestataire?.Entreprise?.Code,
            Tjm = c.Tjm,
            NombreJours = c.NombreJours,
            MontantTotal = c.MontantTotal,
            DateDebut = c.DateDebut,
            DateFin = c.DateFin,
            RepartitionAuto = repartitionAuto,
            Annees = annees,
            JoursConsommes = joursConsommes,
            MontantConsomme = montantConsomme,
            JoursRestants = c.NombreJours - joursConsommes,
            MontantRestant = c.MontantTotal - montantConsomme
        };
    }

    //   Montant au prorata du contrat (= jours × TJM quand Montant = Jours × TJM)
    private static decimal MontantPourJours(Contrat c, decimal jours) =>
        c.NombreJours > 0
            ? Math.Round(c.MontantTotal * jours / c.NombreJours, 2)
            : Math.Round(jours * c.Tjm, 2);

    private static bool MoisChevauche(JourTravail jt, Contrat c)
    {
        var debutMois = DebutMois(jt);
        var finMois = debutMois.AddMonths(1).AddDays(-1);
        return debutMois <= c.DateFin.Date && finMois >= c.DateDebut.Date;
    }

    private static int AnneeDuMois(JourTravail jt) => DebutMois(jt).Year;

    //   "Mois" (YYYY-MM) fait foi, comme pour la validation du quota ; DateTravail en secours
    private static DateTime DebutMois(JourTravail jt)
    {
        var parts = (jt.Mois ?? "").Split('-');
        if (parts.Length == 2 && int.TryParse(parts[0], out var annee) && int.TryParse(parts[1], out var mois)
            && mois is >= 1 and <= 12)
            return new DateTime(annee, mois, 1);
        return new DateTime(jt.DateTravail.Year, jt.DateTravail.Month, 1);
    }
}
