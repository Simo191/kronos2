using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Application.Users.Queries.GetUserByEmail;
using GestionPrestataires.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GestionPrestataires.API.Controllers;

/// <summary>
///   Centre de notifications "façon Facebook" : agrège tout ce qui expire bientôt
///   (orange) ou est déjà expiré (rouge) :
///   - Contrats actifs
///   - Comptes techniques (ADM, ADML, VPN…)
///
///   Filtrage selon rôle :
///   - Admin / Manager : voient les expirations de TOUS les prestataires
///   - User            : voient uniquement les leurs (matché via UserId du JWT)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    //   Fenêtre d'alerte : on remonte ce qui expire dans <= 30 jours OU est déjà expiré.
    private const int WarningWindowDays = 30;
    //   Seuil "quota presque épuisé" : alerte orange si jours restants <= 5.
    private const decimal QuotaWarningThreshold = 5m;

    private readonly IContratRepository _contratRepo;
    private readonly IJourTravailRepository _jourTravailRepo;
    private readonly IPrestataireCredentialRepository _credentialRepo;
    private readonly IPrestataireRepository _prestataireRepo;
    private readonly IMediator _mediator;

    public NotificationsController(
        IContratRepository contratRepo,
        IJourTravailRepository jourTravailRepo,
        IPrestataireCredentialRepository credentialRepo,
        IPrestataireRepository prestataireRepo,
        IMediator mediator)
    {
        _contratRepo = contratRepo;
        _jourTravailRepo = jourTravailRepo;
        _credentialRepo = credentialRepo;
        _prestataireRepo = prestataireRepo;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            //   Restriction côté User : on identifie son prestataireId
            Guid? filterPrestataireId = null;
            if (userRole == "User")
            {
                if (string.IsNullOrEmpty(userEmail))
                    return Ok(Array.Empty<NotificationDto>());

                var userInfo = await _mediator.Send(new GetUserByEmailQuery(userEmail));
                if (userInfo?.Prestataire == null)
                    return Ok(Array.Empty<NotificationDto>());

                filterPrestataireId = userInfo.Prestataire.Id;
            }

            var now = DateTime.UtcNow;
            var limit = now.AddDays(WarningWindowDays);
            var notifications = new List<NotificationDto>();

            // ───────────────── Contrats ─────────────────
            //   Un contrat peut expirer de DEUX façons :
            //   1) Par DATE : DateFin atteint / dépassé
            //   2) Par QUOTA : jours consommés >= NombreJours
            //   On charge donc TOUS les contrats actifs (pas de filtre DateFin),
            //   puis on évalue les deux critères et on garde celui qui alerte.
            var contratsQuery = _contratRepo.GetAllAsync()
                .Include(c => c.Prestataire)
                .Include(c => c.TypeContrat)
                .Where(c => c.Active);

            if (filterPrestataireId.HasValue)
                contratsQuery = contratsQuery.Where(c => c.PrestataireId == filterPrestataireId.Value);

            var contrats = await contratsQuery.ToListAsync();

            //   Jours travaillés (pour calcul du quota consommé) — chargés une seule fois
            var joursTravail = await _jourTravailRepo.GetAllAsync();

            foreach (var c in contrats)
            {
                var libelle = c.TypeContrat?.Libelle is { Length: > 0 } lbl ? lbl : "Contrat";
                var prestaName = c.Prestataire == null
                    ? ""
                    : $"{c.Prestataire.Prenom} {c.Prestataire.Nom}".Trim();

                // ─── Critère #1 : DATE DE FIN ───
                var daysToDateFin = (int)Math.Floor((c.DateFin.Date - now.Date).TotalDays);
                bool dateExpired = daysToDateFin < 0;
                bool dateSoon    = !dateExpired && daysToDateFin <= WarningWindowDays;

                if (dateExpired || dateSoon)
                {
                    notifications.Add(new NotificationDto
                    {
                        Id = $"contrat-{c.Id}-date",
                        Kind = NotificationKind.ContratDate,
                        Severity = dateExpired ? NotificationSeverity.Danger : NotificationSeverity.Warning,
                        PrestataireId = c.PrestataireId,
                        PrestataireName = prestaName,
                        Title = $"Fin de contrat — {libelle}",
                        Subtitle = dateExpired
                            ? $"Expiré depuis {Math.Abs(daysToDateFin)} jour(s)"
                            : (daysToDateFin == 0 ? "Expire aujourd'hui" : $"Expire dans {daysToDateFin} jour(s)"),
                        ExpiresAt = c.DateFin,
                        DaysUntilExpiry = daysToDateFin
                    });
                }

                // ─── Critère #2 : QUOTA DE JOURS ───
                var joursUtilises = joursTravail
                    .Where(jt => jt.PrestataireId == c.PrestataireId
                              && jt.DateTravail >= c.DateDebut
                              && jt.DateTravail <= c.DateFin)
                    .Sum(jt => jt.NombreJours);
                var joursRestants = c.NombreJours - joursUtilises;
                bool quotaExpired = joursRestants <= 0;
                bool quotaSoon    = !quotaExpired && joursRestants <= QuotaWarningThreshold;

                if (quotaExpired || quotaSoon)
                {
                    notifications.Add(new NotificationDto
                    {
                        Id = $"contrat-{c.Id}-quota",
                        Kind = NotificationKind.ContratQuota,
                        Severity = quotaExpired ? NotificationSeverity.Danger : NotificationSeverity.Warning,
                        PrestataireId = c.PrestataireId,
                        PrestataireName = prestaName,
                        Title = $"Quota contrat — {libelle}",
                        Subtitle = quotaExpired
                            ? $"Quota épuisé ({joursUtilises:0.##} / {c.NombreJours:0.##} jours)"
                            : $"Plus que {joursRestants:0.##} j sur {c.NombreJours:0.##}",
                        //   Pour le tri / cohérence : on stocke la date de fin
                        ExpiresAt = c.DateFin,
                        //   On expose les jours restants comme métrique d'urgence quota.
                        //   Négatif si quota dépassé.
                        DaysUntilExpiry = (int)Math.Floor((double)joursRestants)
                    });
                }
            }

            // ───────────────── Credentials ─────────────────
            //   Repo n'a pas de "GetAll" : on requête le DbSet via une nouvelle méthode.
            //   Plus simple ici : on récupère ceux du prestataire ciblé OU tous (Admin/Manager).
            List<Domain.Entities.PrestataireCredential> creds;
            if (filterPrestataireId.HasValue)
            {
                creds = (await _credentialRepo.GetByPrestataireIdAsync(filterPrestataireId.Value))
                    .Where(x => x.ExpiresAt.HasValue && x.ExpiresAt.Value <= limit)
                    .ToList();
            }
            else
            {
                //   Admin / Manager : on parcourt tous les prestataires (peu nombreux : O(n))
                var allPrestataires = await _prestataireRepo.GetAllAsync(includeInactive: true);
                creds = new List<Domain.Entities.PrestataireCredential>();
                foreach (var p in allPrestataires)
                {
                    var list = await _credentialRepo.GetByPrestataireIdAsync(p.Id);
                    creds.AddRange(list.Where(c => c.ExpiresAt.HasValue && c.ExpiresAt.Value <= limit));
                }
            }

            //   Charger les prestataires liés (nom) si Admin/Manager — déjà fait
            //   pour le cas User puisqu'on connaît son nom. Optimisation : map en mémoire.
            var prestaIndex = (await _prestataireRepo.GetAllAsync(includeInactive: true))
                .ToDictionary(p => p.Id, p => $"{p.Prenom} {p.Nom}".Trim());

            foreach (var c in creds)
            {
                if (!c.ExpiresAt.HasValue) continue;
                var days = (int)Math.Floor((c.ExpiresAt.Value.Date - now.Date).TotalDays);
                notifications.Add(new NotificationDto
                {
                    Id = $"cred-{c.Id}",
                    Kind = NotificationKind.Credential,
                    Severity = days < 0 ? NotificationSeverity.Danger : NotificationSeverity.Warning,
                    PrestataireId = c.PrestataireId,
                    PrestataireName = prestaIndex.TryGetValue(c.PrestataireId, out var n) ? n : "",
                    Title = $"Compte {c.Label} — {c.AccountName}",
                    Subtitle = days < 0
                        ? $"Expiré depuis {Math.Abs(days)} jour(s)"
                        : (days == 0 ? "Expire aujourd'hui" : $"Expire dans {days} jour(s)"),
                    ExpiresAt = c.ExpiresAt.Value,
                    DaysUntilExpiry = days
                });
            }

            //   Tri : expirés d'abord (par ancienneté décroissante), puis bientôt
            //   (par proximité croissante).
            var sorted = notifications
                .OrderBy(n => n.DaysUntilExpiry)  // négatifs d'abord (plus expirés en premier)
                .ToList();

            return Ok(sorted);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }
}
