using GestionPrestataires.Application.Estimations.Commands.CreateEstimation;
using GestionPrestataires.Application.Estimations.Commands.DeleteEstimation;
using GestionPrestataires.Application.Estimations.Commands.UpdateEstimation;
using GestionPrestataires.Application.Estimations.Queries.GetAllEstimations;
using GestionPrestataires.Application.Estimations.Queries.GetEstimationByAnnee;
using GestionPrestataires.Application.Estimations.Queries.GetEstimationById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionPrestataires.API.Controllers;

/// <summary>
/// Contrôleur pour les estimations - Pattern CQRS avec MediatR
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class EstimationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EstimationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Récupérer toutes les estimations
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var query = new GetAllEstimationsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }

    /// <summary>
    /// Récupérer une estimation par ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetEstimationByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { message = "Estimation introuvable" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }

    /// <summary>
    /// Récupérer une estimation par année
    /// </summary>
    [HttpGet("annee/{annee}")]
    public async Task<IActionResult> GetByAnnee(int annee)
    {
        try
        {
            var query = new GetEstimationByAnneeQuery(annee);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { message = $"Aucune estimation trouvée pour l'année {annee}" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }

    /// <summary>
    /// Créer une nouvelle estimation
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEstimationCommand command)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Utilisateur non identifié");

            command.CreePar = Guid.Parse(userId);

            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }

    /// <summary>
    /// Mettre à jour une estimation
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEstimationCommand command)
    {
        try
        {
            command.Id = id;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
                command.ModifiePar = Guid.Parse(userId);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }

    /// <summary>
    /// Supprimer une estimation
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteEstimationCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }
}