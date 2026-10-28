using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionPrestataires.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EntreprisesController : ControllerBase
{
    private readonly IEntrepriseRepository _repo;

    public EntreprisesController(IEntrepriseRepository repo) => _repo = repo;

    // ─── Lecture autorisée à tous les rôles authentifiés
    // (utilisée par le select dans le modal Prestataire) ───
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var list = await _repo.GetAllAsync(includeInactive);
        return Ok(list.Select(MapToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? NotFound() : Ok(MapToDto(e));
    }

    // ─── CRUD réservé Admin/Manager ───
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateEntrepriseDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nom))
            return BadRequest(new { message = "Le nom est obligatoire." });

        var now = DateTime.UtcNow;
        var e = new Entreprise
        {
            Id = Guid.NewGuid(),
            Nom = dto.Nom.Trim(),
            Code = string.IsNullOrWhiteSpace(dto.Code) ? null : dto.Code.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _repo.AddAsync(e);
        return CreatedAtAction(nameof(GetById), new { id = e.Id }, MapToDto(e));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEntrepriseDto dto)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Nom))
            return BadRequest(new { message = "Le nom est obligatoire." });

        e.Nom = dto.Nom.Trim();
        e.Code = string.IsNullOrWhiteSpace(dto.Code) ? null : dto.Code.Trim();
        e.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        e.IsActive = dto.IsActive;
        e.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(e);
        return Ok(MapToDto(e));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }

    private static EntrepriseDto MapToDto(Entreprise e) => new()
    {
        Id = e.Id, Nom = e.Nom, Code = e.Code,
        Description = e.Description, IsActive = e.IsActive,
        CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt
    };
}
