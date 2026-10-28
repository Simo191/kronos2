using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using GestionPrestataires.Application.MoisVerrouilles.Commands.VerrouillerMois;
using GestionPrestataires.Application.MoisVerrouilles.Commands.DeverrouillerMois;
using GestionPrestataires.Application.MoisVerrouilles.Queries.GetAllMoisVerrouilles;
using System.Security.Claims;

namespace GestionPrestataires.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MoisVerrouillesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MoisVerrouillesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Récupérer tous les mois verrouillés
    /// Accessible à tous les utilisateurs authentifiés
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var query = new GetAllMoisVerrouillesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur GetAll MoisVerrouilles: {ex.Message}");
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }

    /// <summary>
    /// Verrouiller un mois
    /// Seulement Admin et Manager
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Verrouiller([FromBody] VerrouillerMoisRequest request)
    {
        try
        {
            // Récupérer l'ID de l'utilisateur connecté
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Utilisateur non identifié");

            var command = new VerrouillerMoisCommand(
                request.Mois,
                Guid.Parse(userId),
                request.Commentaire
            );

            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id }, new { id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Déverrouiller un mois
    /// Seulement Admin et Manager
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Deverrouiller(Guid id)
    {
        try
        {
            var command = new DeverrouillerMoisCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class VerrouillerMoisRequest
{
    public string Mois { get; set; } = string.Empty;
    public string? Commentaire { get; set; }
}