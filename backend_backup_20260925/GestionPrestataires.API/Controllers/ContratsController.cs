using GestionPrestataires.Application.Contrats.Commands.CreateContrat;
using GestionPrestataires.Application.Contrats.Commands.DeleteContrat;
using GestionPrestataires.Application.Contrats.Commands.UpdateContrat;
using GestionPrestataires.Application.Contrats.Commands.UpdateContratDocument;
using GestionPrestataires.Application.Contrats.Queries.GetAllContrats;
using GestionPrestataires.Application.Contrats.Queries.GetContratById;
using GestionPrestataires.Application.Users.Queries.GetUserByEmail;
using GestionPrestataires.Application.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionPrestataires.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]  //   Authentification requise
public class ContratsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContratsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///   SÉCURISÉ : Filtrage ET DTO selon rôle
    /// - Admin/Manager : Tous les contrats avec TJM/MontantTotal
    /// - User : Ses contrats SANS TJM/MontantTotal
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            //   Récupérer le rôle et l'email de l'utilisateur
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var prestataireId = User.FindFirst("PrestataireId")?.Value;

            Console.WriteLine($"🔍 GetAll Contrats - Role: {userRole}, Email: {userEmail}, PrestataireId: {prestataireId}");

            //   TON CODE PRÉSERVÉ : Si pas de prestataireId dans JWT, chercher par email
            if (string.IsNullOrEmpty(prestataireId) && userRole == "User")
            {
                var queryUser = new GetUserByEmailQuery(userEmail ?? "");
                var resultUser = await _mediator.Send(queryUser);
                if (resultUser != null)
                    prestataireId = resultUser.Prestataire!.Id.ToString();

                Console.WriteLine($"  PrestataireId trouvé via email: {prestataireId}");
            }

            //   CORRECTION : Passer UserRole et PrestataireId au Query
            var query = new GetAllContratsQuery(userRole, prestataireId);
            var result = await _mediator.Send(query);

            //   SUPPRIMÉ : Le filtrage est maintenant fait dans le Handler
            // Le Handler retourne déjà :
            // - ContratDto pour Admin/Manager (avec TJM)
            // - ContratDtoRestricted pour User (sans TJM)

            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur GetAll Contrats: {ex.Message}");
            return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetContratByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        //   SÉCURITÉ : User ne peut voir que ses contrats
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var prestataireId = User.FindFirst("PrestataireId")?.Value;

        //   TON CODE PRÉSERVÉ : Si pas de prestataireId dans JWT, chercher par email
        if (string.IsNullOrEmpty(prestataireId))
        {
            var queryUser = new GetUserByEmailQuery(userEmail ?? "");
            var resultUser = await _mediator.Send(queryUser);
            if (resultUser != null)
                prestataireId = resultUser.Prestataire!.Id.ToString();
        }

        if (userRole == "User" && !string.IsNullOrEmpty(prestataireId))
        {
            if (result.PrestataireId.ToString() != prestataireId)
                return Forbid();  // ❌ Pas ton contrat !
        }

        //   Si User, masquer TJM/MontantTotal (objet anonyme)
        if (userRole == "User")
        {
            var restricted = new
            {
                result.Id,
                result.PrestataireId,
                result.TypeContratId,
                result.DateDebut,
                result.DateFin,
                result.NombreJours,
                // Tjm = MASQUÉ
                // MontantTotal = MASQUÉ
                result.JoursUtilises,
                result.CreatedAt,
                result.UpdatedAt,
                Prestataire = result.Prestataire != null ? new
                {
                    result.Prestataire.Id,
                    result.Prestataire.Nom,
                    result.Prestataire.Prenom,
                    result.Prestataire.Email,
                    result.Prestataire.Telephone
                    // Tjm = MASQUÉ
                } : null,
                result.TypeContrat
            };
            return Ok(restricted);
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]  //   Seuls Admin/Manager peuvent créer
    public async Task<IActionResult> Create([FromBody] CreateContratCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]  //   Seuls Admin/Manager peuvent modifier
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContratCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]  //   Seuls Admin/Manager peuvent supprimer
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteContratCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }/// <summary>
     /// ✅ NOUVEAU : Upload document pour un contrat
     /// Stockage : wwwroot/doc/contrat/{NomPrestataire}/{NomPrestataire_DateDebut_Random.ext}
     /// </summary>
    [HttpPost("{id}/upload")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UploadDocument(Guid id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Aucun fichier fourni." });

        // Taille max 10 Mo
        if (file.Length > 10 * 1024 * 1024)
            return BadRequest(new { message = "Le fichier ne doit pas dépasser 10 Mo." });

        // Types autorisés
        var allowedTypes = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedTypes.Contains(ext))
            return BadRequest(new { message = "Format non autorisé." });

        // Récupérer le contrat avec prestataire
        var query = new GetContratByIdQuery(id);
        var contrat = await _mediator.Send(query);
        if (contrat == null) return NotFound();

        // Construire le nom du prestataire (slug propre)
        var nomPrestataire = $"{contrat.Prestataire?.Nom ?? "inconnu"}_{contrat.Prestataire?.Prenom ?? ""}";
        nomPrestataire = nomPrestataire.Trim().Replace(" ", "_").Replace("/", "_").Replace("\\", "_");

        // Nom du fichier : NomPrestataire_DateDebut_Random.ext
        var dateDebutStr = contrat.DateDebut.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);
        var fileName = $"{nomPrestataire}_{dateDebutStr}_{random}{ext}";

        // Chemin : wwwroot/doc/contrat/{NomPrestataire}/
        var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var folder = Path.Combine(wwwroot, "doc", "contrat", nomPrestataire);
        Directory.CreateDirectory(folder);

        var filePath = Path.Combine(folder, fileName);

        // Écrire le fichier
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // URL relative stockée en base
        var documentUrl = $"doc/contrat/{nomPrestataire}/{fileName}";

        // Mettre à jour le contrat
        var updateDocCommand = new UpdateContratDocumentCommand(id, documentUrl);
        await _mediator.Send(updateDocCommand);

        return Ok(new { documentUrl });
    }
}