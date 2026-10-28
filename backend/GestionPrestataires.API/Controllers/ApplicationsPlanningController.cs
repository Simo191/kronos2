using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionPrestataires.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationsPlanningController : ControllerBase
{
    private readonly IApplicationPlanningRepository _repository;
    private readonly IApplicationPrestataireAssignmentRepository _repositoryApplicationPrestataireAssignmentRepository;
    private readonly IApplicationServerRepository _serverRepository;

    public ApplicationsPlanningController(IApplicationPlanningRepository repository,
        IApplicationPrestataireAssignmentRepository repositoryApplicationPrestataireAssignmentRepository,
        IApplicationServerRepository serverRepository)
    {
        _repository = repository;
        _repositoryApplicationPrestataireAssignmentRepository = repositoryApplicationPrestataireAssignmentRepository;
        _serverRepository = serverRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var apps = await _repository.GetAllAsync();
            var dtos = apps.Select(MapToDto).ToList();
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var app = await _repository.GetByIdAsync(id);
            if (app == null) return NotFound(new { message = "Application introuvable" });
            return Ok(MapToDto(app));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateApplicationPlanningDto dto)
    {
        try
        {
            var app = new ApplicationPlanning
            {
                Id = Guid.NewGuid(),
                Nom = dto.Nom,
                Description = dto.Description,
                DateDebut = DateTime.SpecifyKind(dto.DateDebut, DateTimeKind.Utc),
                DateFin = DateTime.SpecifyKind(dto.DateFin, DateTimeKind.Utc),
                Statut = dto.Statut,
                Priorite = dto.Priorite,
                Couleur = dto.Couleur,
                Avancement = dto.Avancement,
                Ordre = dto.Ordre,
                DateCreation = DateTime.UtcNow,
                DateModification = DateTime.UtcNow,
                Assignments = dto.Prestataires.Select(p => new ApplicationPrestataireAssignment
                {
                    Id = Guid.NewGuid(),
                    PrestataireId = p.PrestataireId,
                    Role = p.Role
                }).ToList(),
                //   Serveurs / environnements (DEV / RECETTE / PROD / INT / AUTRE)
                Servers = (dto.Servers ?? new()).Select((s, idx) => new ApplicationServer
                {
                    Id = Guid.NewGuid(),
                    Type = NormalizeServerType(s.Type),
                    Identifiant = (s.Identifiant ?? string.Empty).Trim(),
                    Libelle = string.IsNullOrWhiteSpace(s.Libelle) ? null : s.Libelle.Trim(),
                    Note = string.IsNullOrWhiteSpace(s.Note) ? null : s.Note.Trim(),
                    Ordre = s.Ordre == 0 ? idx : s.Ordre
                })
                .Where(s => !string.IsNullOrWhiteSpace(s.Identifiant))
                .ToList()
            };

            var created = await _repository.AddAsync(app);
            var result = await _repository.GetByIdAsync(created.Id);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(result!));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateApplicationPlanningDto dto)
    {
        try
        {
            var app = await _repository.GetByIdAsync(id);
            if (app == null) return NotFound(new { message = "Application introuvable" });

            app.Nom = dto.Nom;
            app.Description = dto.Description;
            app.DateDebut = DateTime.SpecifyKind(dto.DateDebut, DateTimeKind.Utc);
            app.DateFin = DateTime.SpecifyKind(dto.DateFin, DateTimeKind.Utc);
            app.Statut = dto.Statut;
            app.Priorite = dto.Priorite;
            app.Couleur = dto.Couleur;
            app.Avancement = dto.Avancement;
            app.Ordre = dto.Ordre;
            app.DateModification = DateTime.UtcNow;

            // Remplacer les assignments
            app.Assignments.Clear();


            List<ApplicationPrestataireAssignment> listpresta = dto.Prestataires
           .Select(p => new ApplicationPrestataireAssignment
           {
               Id = Guid.NewGuid(),
               ApplicationId = id,
               PrestataireId = p.PrestataireId,
               Role = p.Role
           })
           .ToList();

            //   Préparer la nouvelle liste de serveurs (delete + insert via repo dédié,
            //   même pattern que les Assignments → évite les conflits de tracking EF).
            //   IMPORTANT : on NE fait PAS `app.Servers.Clear()` ici. Si on le faisait,
            //   EF marquerait les enfants tracked comme Deleted. Puis ExecuteDeleteAsync
            //   les supprimerait en DB → au SaveChanges suivant EF tenterait de re-DELETE
            //   des lignes déjà supprimées (0 rows affected → OptimisticConcurrencyException).
            //   dto.Servers == null (champ absent, ex. réordonnancement drag & drop) :
            //   on NE touche PAS aux serveurs existants.
            var remplacerServeurs = dto.Servers != null;
            var newServers = (dto.Servers ?? new())
                .Select((s, idx) => new ApplicationServer
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = id,
                    Type = NormalizeServerType(s.Type),
                    Identifiant = (s.Identifiant ?? string.Empty).Trim(),
                    Libelle = string.IsNullOrWhiteSpace(s.Libelle) ? null : s.Libelle.Trim(),
                    Note = string.IsNullOrWhiteSpace(s.Note) ? null : s.Note.Trim(),
                    Ordre = s.Ordre == 0 ? idx : s.Ordre
                })
                .Where(s => !string.IsNullOrWhiteSpace(s.Identifiant))
                .ToList();

            await _repositoryApplicationPrestataireAssignmentRepository.DeleteAllAsync(id);
            if (remplacerServeurs)
                await _serverRepository.DeleteAllAsync(id);
            await _repository.UpdateAsync(app);
            await _repositoryApplicationPrestataireAssignmentRepository.AddRangeAsync(listpresta);
            if (remplacerServeurs)
                await _serverRepository.AddRangeAsync(newServers);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }

    private static ApplicationPlanningDto MapToDto(ApplicationPlanning app) => new()
    {
        Id = app.Id,
        Nom = app.Nom,
        Description = app.Description,
        DateDebut = app.DateDebut,
        DateFin = app.DateFin,
        Statut = app.Statut,
        Priorite = app.Priorite,
        Couleur = app.Couleur,
        Avancement = app.Avancement,
        DateCreation = app.DateCreation,
        DateModification = app.DateModification,
        Ordre = app.Ordre,
        Prestataires = app.Assignments.Select(a => new AssignmentDto
        {
            PrestataireId = a.PrestataireId,
            //   On inclut " (CODE/NOM)" si l'entreprise est définie — visible dans
            //   le Gantt + dropdowns + tooltips sans toucher le front.
            NomComplet = a.Prestataire == null
                ? ""
                : a.Prestataire.Entreprise == null
                    ? $"{a.Prestataire.Nom} {a.Prestataire.Prenom}"
                    : $"{a.Prestataire.Nom} {a.Prestataire.Prenom} ({a.Prestataire.Entreprise.Code ?? a.Prestataire.Entreprise.Nom})",
            Role = a.Role
        }).ToList(),
        Servers = app.Servers
            .OrderBy(s => s.Ordre)
            .Select(s => new ApplicationServerDto
            {
                Id = s.Id,
                Type = s.Type,
                Identifiant = s.Identifiant,
                Libelle = s.Libelle,
                Note = s.Note,
                Ordre = s.Ordre
            })
            .ToList()
    };

    //   Normalise le type d'environnement reçu du front (anti-faute de frappe).
    private static readonly HashSet<string> AllowedServerTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "DEV", "RECETTE", "PROD", "INT", "AUTRE"
    };

    private static string NormalizeServerType(string? type)
    {
        if (string.IsNullOrWhiteSpace(type)) return "AUTRE";
        var upper = type.Trim().ToUpperInvariant();
        return AllowedServerTypes.Contains(upper) ? upper : "AUTRE";
    }
}