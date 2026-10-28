using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GestionPrestataires.API.Controllers;

/// <summary>
///   Comptes techniques (ADM, ADML, VPN…) d'un prestataire.
///   Mot de passe chiffré AES-256-GCM, jamais retourné sauf via /reveal.
///   Règles d'accès :
///   - Admin / Manager : accès complet (lire, créer, modifier, supprimer, reveal) sur tous les prestataires
///   - User : accès uniquement au prestataire dont UserId == son userId JWT
/// </summary>
[ApiController]
[Route("api/prestataires/{prestataireId:guid}/credentials")]
[Authorize]
public class PrestataireCredentialsController : ControllerBase
{
    private readonly IPrestataireCredentialRepository _repo;
    private readonly IPrestataireRepository _prestataireRepo;
    private readonly IUserRepository _userRepo;
    private readonly IPasswordEncryptionService _encryption;

    public PrestataireCredentialsController(
        IPrestataireCredentialRepository repo,
        IPrestataireRepository prestataireRepo,
        IUserRepository userRepo,
        IPasswordEncryptionService encryption)
    {
        _repo = repo;
        _prestataireRepo = prestataireRepo;
        _userRepo = userRepo;
        _encryption = encryption;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GET : liste des comptes du prestataire (mdp masqué)
    // ─────────────────────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid prestataireId)
    {
        if (!await EnsureAccessAsync(prestataireId)) return Forbid();

        var creds = await _repo.GetByPrestataireIdAsync(prestataireId);
        return Ok(creds.Select(MapToDto));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST : créer un compte (mdp chiffré)
    // ─────────────────────────────────────────────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> Create(Guid prestataireId, [FromBody] CreatePrestataireCredentialDto dto)
    {
        if (!await EnsureAccessAsync(prestataireId)) return Forbid();

        if (string.IsNullOrWhiteSpace(dto.Label) ||
            string.IsNullOrWhiteSpace(dto.AccountName) ||
            string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "Label, AccountName et Password sont obligatoires." });

        var now = DateTime.UtcNow;
        var entity = new PrestataireCredential
        {
            Id = Guid.NewGuid(),
            PrestataireId = prestataireId,
            Label = dto.Label.Trim(),
            AccountName = dto.AccountName.Trim(),
            EncryptedPassword = _encryption.Encrypt(dto.Password),
            ExpiresAt = dto.ExpiresAt.HasValue
                ? DateTime.SpecifyKind(dto.ExpiresAt.Value, DateTimeKind.Utc)
                : null,
            Note = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetAll), new { prestataireId }, MapToDto(entity));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PUT : mettre à jour ; mdp vide = on garde l'ancien chiffré
    // ─────────────────────────────────────────────────────────────────────────
    [HttpPut("{credentialId:guid}")]
    public async Task<IActionResult> Update(Guid prestataireId, Guid credentialId, [FromBody] UpdatePrestataireCredentialDto dto)
    {
        if (!await EnsureAccessAsync(prestataireId)) return Forbid();

        var entity = await _repo.GetByIdAsync(credentialId);
        if (entity == null || entity.PrestataireId != prestataireId)
            return NotFound(new { message = "Compte introuvable." });

        if (string.IsNullOrWhiteSpace(dto.Label) || string.IsNullOrWhiteSpace(dto.AccountName))
            return BadRequest(new { message = "Label et AccountName sont obligatoires." });

        entity.Label = dto.Label.Trim();
        entity.AccountName = dto.AccountName.Trim();
        entity.ExpiresAt = dto.ExpiresAt.HasValue
            ? DateTime.SpecifyKind(dto.ExpiresAt.Value, DateTimeKind.Utc)
            : null;
        entity.Note = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim();
        entity.UpdatedAt = DateTime.UtcNow;

        //   Mot de passe : on ne le re-chiffre QUE si l'appelant en a fourni un.
        if (!string.IsNullOrWhiteSpace(dto.Password))
            entity.EncryptedPassword = _encryption.Encrypt(dto.Password);

        await _repo.UpdateAsync(entity);
        return Ok(MapToDto(entity));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // DELETE
    // ─────────────────────────────────────────────────────────────────────────
    [HttpDelete("{credentialId:guid}")]
    public async Task<IActionResult> Delete(Guid prestataireId, Guid credentialId)
    {
        if (!await EnsureAccessAsync(prestataireId)) return Forbid();

        var entity = await _repo.GetByIdAsync(credentialId);
        if (entity == null || entity.PrestataireId != prestataireId)
            return NotFound();

        await _repo.DeleteAsync(credentialId);
        return NoContent();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST /reveal : exige le mdp utilisateur courant. Retour : mdp en clair.
    // ─────────────────────────────────────────────────────────────────────────
    [HttpPost("{credentialId:guid}/reveal")]
    public async Task<IActionResult> Reveal(Guid prestataireId, Guid credentialId, [FromBody] RevealCredentialRequestDto body)
    {
        if (!await EnsureAccessAsync(prestataireId)) return Forbid();

        if (string.IsNullOrEmpty(body?.UserPassword))
            return BadRequest(new { message = "Mot de passe requis." });

        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return Unauthorized();

        //   Vérifie le mdp utilisateur courant avant tout déchiffrement
        if (!BCrypt.Net.BCrypt.Verify(body.UserPassword, user.PasswordHash))
            return Unauthorized(new { message = "Mot de passe incorrect." });

        var entity = await _repo.GetByIdAsync(credentialId);
        if (entity == null || entity.PrestataireId != prestataireId)
            return NotFound(new { message = "Compte introuvable." });

        try
        {
            var plain = _encryption.Decrypt(entity.EncryptedPassword);
            return Ok(new RevealCredentialResponseDto { Id = entity.Id, Password = plain });
        }
        catch (Exception)
        {
            //   Tag GCM invalide / clé incorrecte / payload corrompu
            return StatusCode(500, new { message = "Impossible de déchiffrer le mot de passe (clé serveur changée ?)." });
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────────────

    private Guid GetCurrentUserId()
    {
        var idStr = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                 ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(idStr, out var id) ? id : Guid.Empty;
    }

    private string? GetCurrentRole() => User.FindFirst(ClaimTypes.Role)?.Value;

    /// <summary>
    ///   Renvoie true si l'utilisateur courant a le droit d'accéder aux credentials
    ///   du prestataire indiqué. Admin/Manager : oui ; sinon : seulement si owner.
    /// </summary>
    private async Task<bool> EnsureAccessAsync(Guid prestataireId)
    {
        var role = GetCurrentRole();
        if (role == "Admin" || role == "Manager") return true;

        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return false;

        var prestataire = await _prestataireRepo.GetByIdAsync(prestataireId);
        return prestataire?.UserId == userId;
    }

    private static PrestataireCredentialDto MapToDto(PrestataireCredential c) => new()
    {
        Id = c.Id,
        PrestataireId = c.PrestataireId,
        Label = c.Label,
        AccountName = c.AccountName,
        ExpiresAt = c.ExpiresAt,
        Note = c.Note,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
