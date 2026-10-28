using GestionPrestataires.Application.Auth.Commands.ChangePassword;
using GestionPrestataires.Application.Authentication.Commands.Login;
using GestionPrestataires.Application.Authentication.Commands.Register;
using GestionPrestataires.Application.Authentication.Commands.TompoPassword;
using GestionPrestataires.Application.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionPrestataires.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]  //   OBLIGATOIRE : Authentification requise

public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///   Login - Accessible sans authentification
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    ///   Register - Accessible sans authentification
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        try
        {
            var userId = await _mediator.Send(command);
            return Ok(new { userId, message = "Inscription réussie" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    /// <summary>
    /// Changer le mot de passe de l'utilisateur connecté
    /// </summary>
    [HttpPost("change-password")]
    [Authorize(Roles = "Admin,Manager,User")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            Guid resolvedUserId;

            // 1️⃣ Si l’ID est fourni dans la requête
            if (request.IdUser.HasValue && request.IdUser.Value != Guid.Empty)
            {
                resolvedUserId = request.IdUser.Value;
                var query = new GetUserByIdQuery(request.IdUser.Value);
                var result = await _mediator.Send(query);
            }
            else
            {
                // 2️⃣ Sinon récupérer depuis le token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userIdClaim) ||
                    !Guid.TryParse(userIdClaim, out resolvedUserId))
                {
                    return Unauthorized(new { message = "Utilisateur non authentifié" });
                }
            }

            // 3️⃣ Commande avec Guid garanti valide
            var command = new ChangePasswordCommand(
                resolvedUserId,
                request.CurrentPassword,
                request.NewPassword
            );


            await _mediator.Send(command);

            return Ok(new { message = "Mot de passe modifié avec succès" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erreur lors du changement de mot de passe", error = ex.Message });
        }
    }
    [HttpPost("tempo-password")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> TompoPassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            Guid resolvedUserId = Guid.Empty;

            // 1️⃣ Si l’ID est fourni dans la requête
            if (request.IdUser.HasValue && request.IdUser.Value != Guid.Empty)
            {
                resolvedUserId = request.IdUser.Value;
            }
            else
                return BadRequest(new { message = "Erreur lors du changement de mot de passe" });


            // 3️⃣ Commande avec Guid garanti valide
            var command = new TompoPasswordCommand(
                resolvedUserId,
                request.CurrentPassword,
                request.NewPassword
            );


            await _mediator.Send(command);

            return Ok(new { message = "Mot de passe modifié avec succès" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erreur lors du changement de mot de passe", error = ex.Message });
        }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public Guid? IdUser { get; set; } = Guid.Empty;
    }
}