using GestionPrestataires.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Application.JoursTravail.Commands.UpdateJourTravail;

public class UpdateJourTravailCommandHandler : IRequestHandler<UpdateJourTravailCommand>
{
    private readonly IJourTravailRepository _repository;
    private readonly IContratRepository _contratRepository;
    private readonly IMoisVerrouillRepository _moisVerrouillRepository;

    public UpdateJourTravailCommandHandler(
        IJourTravailRepository repository,
        IContratRepository contratRepository,
        IMoisVerrouillRepository moisVerrouillRepository    )
    {
        _repository = repository;
        _contratRepository = contratRepository;
        _moisVerrouillRepository = moisVerrouillRepository;
    }

    public async Task Handle(UpdateJourTravailCommand request, CancellationToken cancellationToken)
    {
        //   Vérifier si le mois est verrouillé
        var estVerrouille = await _moisVerrouillRepository.IsMoisVerrouille(request.Mois);
        if (estVerrouille)
            throw new Exception($"Le mois {request.Mois} est verrouillé. Impossible de modifier des jours de travail.");


        // Récupérer le jour de travail existant
        var jourTravail = await _repository.GetByIdAsync(request.Id);
        if (jourTravail == null)
            throw new Exception("Jour de travail introuvable");

        //   Récupérer le TJM depuis le contrat actif
        var tjmFinal = request.Tjm;

        // Si TJM = 0 dans la requête (cas User), récupérer depuis le contrat
        if (request.Tjm == 0)
        {
            var contrat = await FindActiveContratForMonth(request.PrestataireId, request.Mois);

            if (contrat == null)
                throw new Exception($"Aucun contrat actif trouvé pour le prestataire sur le mois {request.Mois}");

            tjmFinal = contrat.Tjm;
        }

        //   Valider que le TJM final est valide
        if (tjmFinal <= 0)
            throw new Exception("TJM invalide ou non défini dans le contrat");

        //   Valider le quota du contrat
        var contratActif = await FindActiveContratForMonth(request.PrestataireId, request.Mois);
        if (contratActif != null)
        {
            await ValidateQuota(contratActif, request.NombreJours, request.Id);
        }

        // Mettre à jour le jour de travail
        var datejour = Convert.ToDateTime(request.Mois);
        var dateTravail = new DateTime(datejour.Year, datejour.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        jourTravail.PrestataireId = request.PrestataireId;
        jourTravail.Mois = request.Mois;
        jourTravail.Tjm = tjmFinal; //   Utiliser le TJM récupéré du contrat
        jourTravail.NombreJours = request.NombreJours;
        jourTravail.Description = request.Description;
        jourTravail.DateTravail = dateTravail;

        await _repository.UpdateAsync(jourTravail);
    }

    //   Trouver le contrat actif pour un mois donné
    private async Task<Domain.Entities.Contrat?> FindActiveContratForMonth(Guid prestataireId, string moisYYYYMM)
    {
        var parts = moisYYYYMM.Split('-');
        if (parts.Length != 2 || !int.TryParse(parts[0], out int year) || !int.TryParse(parts[1], out int month))
            return null;

        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1).AddSeconds(-1);

        var contrats = await _contratRepository.GetByPrestataireIdAsync(prestataireId).ToListAsync();

        return contrats
            .Where(c => c.DateDebut <= monthEnd && (c.DateFin == null || c.DateFin >= monthStart))
            .OrderByDescending(c => c.DateDebut)
            .FirstOrDefault();
    }

    //   Valider le quota du contrat
    private async Task ValidateQuota(Domain.Entities.Contrat contrat, decimal nombreJoursSaisi, Guid jourTravailIdToExclude)
    {
        // Récupérer tous les jours du prestataire pour ce contrat
        var allJours = await _repository.GetAllAsync(contrat.PrestataireId);

        // Filtrer les jours dans la période du contrat (exclure le jour en cours de modification)
        var joursInPeriod = allJours
            .Where(j => j.Id != jourTravailIdToExclude)
            .Where(j => IsMonthInContratPeriod(j.Mois, contrat))
            .ToList();

        var joursDejaUtilises = joursInPeriod.Sum(j => j.NombreJours);
        var joursDisponibles = contrat.NombreJours - joursDejaUtilises;

        if (nombreJoursSaisi > joursDisponibles)
        {
            throw new Exception(
                $"Quota dépassé : vous essayez de saisir {nombreJoursSaisi} jour(s) mais il ne reste que {joursDisponibles} jour(s) disponibles sur le contrat " +
                $"(quota: {contrat.NombreJours} jours, déjà utilisés: {joursDejaUtilises} jours)."
            );
        }
    }

    //   Vérifier si un mois est dans la période du contrat
    private bool IsMonthInContratPeriod(string moisYYYYMM, Domain.Entities.Contrat contrat)
    {
        var parts = moisYYYYMM.Split('-');
        if (parts.Length != 2 || !int.TryParse(parts[0], out int year) || !int.TryParse(parts[1], out int month))
            return false;

        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1).AddSeconds(-1);

        return contrat.DateDebut <= monthEnd && (contrat.DateFin == null || contrat.DateFin >= monthStart);
    }
}