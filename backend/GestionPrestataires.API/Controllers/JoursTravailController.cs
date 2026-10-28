using GestionPrestataires.Application.JoursTravail.Commands.CreateJourTravail;
using GestionPrestataires.Application.JoursTravail.Commands.DeleteJourTravail;
using GestionPrestataires.Application.JoursTravail.Commands.UpdateJourTravail;
using GestionPrestataires.Application.JoursTravail.Queries.GetAllJoursTravail;
using GestionPrestataires.Application.Users.Queries.GetUserByEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionPrestataires.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]  //   Authentification requise
public class JoursTravailController : ControllerBase
{
    private readonly IMediator _mediator;

    public JoursTravailController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///   SÉCURISÉ : Filtrage ET DTO selon rôle
    /// - Admin/Manager : Tous les jours avec Tjm/Revenu
    /// - User : Ses jours SANS Tjm/Revenu
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? prestataireId = null,
        [FromQuery] string? mois = null)  //   Mois au lieu de annee
    {
        try
        {
            //   Récupérer le rôle et l'email de l'utilisateur
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userPrestataireId = User.FindFirst("PrestataireId")?.Value;

            Console.WriteLine($"🔍 GetAll JoursTravail - Role: {userRole}, Email: {userEmail}, PrestataireId: {userPrestataireId}");

            //   Si pas de prestataireId dans JWT, chercher par email
            if (string.IsNullOrEmpty(userPrestataireId) && userRole == "User")
            {
                var queryUser = new GetUserByEmailQuery(userEmail ?? "");
                var resultUser = await _mediator.Send(queryUser);
                if (resultUser != null)
                    userPrestataireId = resultUser.Prestataire!.Id.ToString();

                Console.WriteLine($"  PrestataireId trouvé via email: {userPrestataireId}");
            }

            //   Si User, forcer son prestataireId
            if (userRole == "User" && !string.IsNullOrEmpty(userPrestataireId))
            {
                prestataireId = Guid.Parse(userPrestataireId);
            }

            //   Passer UserRole et UserPrestataireId au Query
            var query = new GetAllJoursTravailQuery(
                prestataireId,
                mois,  //   Mois au lieu de annee
                userRole,
                userPrestataireId
            );
            var result = await _mediator.Send(query);

            //   Plus besoin de filtrer ici, le Handler s'en charge !
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur GetAll JoursTravail: {ex.Message}");
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }

    /// <summary>
    ///   User peut créer ses jours
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJourTravailCommand command)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userPrestataireId = User.FindFirst("PrestataireId")?.Value;

            // TODO : Vérifier que le prestataireId du command correspond au User
            // if (userRole == "User" && command.PrestataireId.ToString() != userPrestataireId)
            //     return Forbid();

            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id }, new { id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erreur création", error = ex.Message });
        }
    }

    /// <summary>
    ///   Seuls Admin/Manager peuvent modifier
    /// </summary>
    [HttpPut("{id}")]
    //[Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJourTravailCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///   Seuls Admin/Manager peuvent supprimer
    /// </summary>
    [HttpDelete("{id}")]
    //[Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteJourTravailCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}