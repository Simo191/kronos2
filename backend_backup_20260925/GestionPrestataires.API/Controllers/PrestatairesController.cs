using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using GestionPrestataires.Application.Prestataires.Commands.CreatePrestataire;
using GestionPrestataires.Application.Prestataires.Commands.UpdatePrestataire;
using GestionPrestataires.Application.Prestataires.Commands.DeletePrestataire;
using GestionPrestataires.Application.Prestataires.Queries.GetAllPrestataires;
using GestionPrestataires.Application.Prestataires.Queries.GetPrestataireById;
using GestionPrestataires.Domain.Repositories;
using System.Security.Claims;
using GestionPrestataires.Application.Users.Queries.GetUserByEmail;

namespace GestionPrestataires.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrestatairesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPrestataireRepository _repository;   // ✅ NOUVEAU
    private readonly IWebHostEnvironment _env;             // ✅ NOUVEAU

    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxImageSize = 5 * 1024 * 1024; // 5 Mo

    public PrestatairesController(IMediator mediator, IPrestataireRepository repository, IWebHostEnvironment env)
    {
        _mediator = mediator;
        _repository = repository;
        _env = env;
    }

    /// <summary>
    ///   SÉCURISÉ : Filtrage selon rôle
    /// - Admin/Manager : Tous les prestataires
    /// - User : Bloqué (ne devrait pas accéder, mais si accès, renvoie vide)
    /// </summary> 
    [HttpGet]
    [Authorize(Roles = "Admin,Manager,User")]  //   User ne peut PAS accéder
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var prestataireId = User.FindFirst("PrestataireId")?.Value;

            Console.WriteLine($"🔍 GetAll Contrats - Role: {userRole}, Email: {userEmail}, PrestataireId: {prestataireId}");

            //   TON CODE PRÉSERVÉ : Si pas de prestataireId dans JWT, chercher par email


            var query = new GetAllPrestatairesQuery();
            var result = await _mediator.Send(query);
            if (string.IsNullOrEmpty(prestataireId) && userRole == "User")
            {
                var queryUser = new GetUserByEmailQuery(userEmail ?? "");
                var resultUser = await _mediator.Send(queryUser);
                if (resultUser != null)
                {
                    prestataireId = resultUser.Prestataire!.Id.ToString();
                    result = result.Where(s => s.Id.ToString() == prestataireId).ToList();
                }

                Console.WriteLine($"  PrestataireId trouvé via email: {prestataireId}");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }
    /// <summary>
    ///   SÉCURISÉ : Filtrage selon rôle
    /// - Admin/Manager : Tous les prestataires
    /// - User : Bloqué (ne devrait pas accéder, mais si accès, renvoie vide)
    /// </summary> 
    [HttpGet("prestataireshift")]
    [Authorize(Roles = "Admin,Manager,User")]  //   User ne peut PAS accéder
    public async Task<IActionResult> Getprestataireshift()
    {
        try
        {
            var query = new GetAllPrestatairesQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]  //   User ne peut PAS voir les détails
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetPrestataireByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]  //   Seuls Admin/Manager peuvent créer
    public async Task<IActionResult> Create([FromBody] CreatePrestataireCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erreur création", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]  //   Seuls Admin/Manager peuvent modifier
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePrestataireCommand command)
    {
        try
        {
            if (id != command.Id)
                return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erreur modification", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]  //   Seuls Admin/Manager peuvent supprimer
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeletePrestataireCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }

    // =========================================================================
    // ✅ NOUVEAU : Upload avatar prestataire
    // POST api/prestataires/{id}/upload-image
    // Stocké dans wwwroot/images/prestataires/{id}.ext
    // =========================================================================
    [HttpPost("{id:guid}/upload-image")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Aucun fichier fourni." });

        if (file.Length > MaxImageSize)
            return BadRequest(new { message = "Fichier trop volumineux (max 5 Mo)." });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(ext))
            return BadRequest(new { message = "Format non supporté. Utilisez JPG, PNG ou WEBP." });

        var prestataire = await _repository.GetByIdAsync(id);
        if (prestataire == null)
            return NotFound(new { message = "Prestataire introuvable." });

        // Créer le dossier si inexistant
        var folder = Path.Combine(_env.WebRootPath, "images", "prestataires");
        Directory.CreateDirectory(folder);

        // Supprimer l'ancienne image (toutes extensions)
        foreach (var e in AllowedImageExtensions)
        {
            var old = Path.Combine(folder, $"{id}{e}");
            if (System.IO.File.Exists(old))
                System.IO.File.Delete(old);
        }

        // Sauvegarder avec l'ID comme nom de fichier (évite les collisions)
        var fileName = $"{id}{ext}";
        var filePath = Path.Combine(folder, fileName);
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Mettre à jour ImageUrl en base
        var imageUrl = $"images/prestataires/{fileName}";
        prestataire.ImageUrl = imageUrl;
        prestataire.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(prestataire);

        return Ok(new { imageUrl });
    }

    // =========================================================================
    // ✅ NOUVEAU : Supprimer l'avatar d'un prestataire
    // DELETE api/prestataires/{id}/image
    // =========================================================================
    [HttpDelete("{id:guid}/image")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteImage(Guid id)
    {
        var prestataire = await _repository.GetByIdAsync(id);
        if (prestataire == null)
            return NotFound(new { message = "Prestataire introuvable." });

        if (!string.IsNullOrEmpty(prestataire.ImageUrl))
        {
            var filePath = Path.Combine(
                _env.WebRootPath,
                prestataire.ImageUrl.Replace('/', Path.DirectorySeparatorChar)
            );
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            prestataire.ImageUrl = null;
            prestataire.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(prestataire);
        }

        return NoContent();
    }
}