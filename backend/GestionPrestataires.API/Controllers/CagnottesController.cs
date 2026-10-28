using GestionPrestataires.Application.Cagnottes.Commands.AddSourceBudget;
using GestionPrestataires.Application.Cagnottes.Commands.CreateCagnotte;
using GestionPrestataires.Application.Cagnottes.Commands.DeleteCagnotte;
using GestionPrestataires.Application.Cagnottes.Commands.DeleteSourceBudget;
using GestionPrestataires.Application.Cagnottes.Commands.RecalculerBudget;
using GestionPrestataires.Application.Cagnottes.Commands.UpdateCagnotte;
using GestionPrestataires.Application.Cagnottes.Commands.UpdateSourceBudget;
using GestionPrestataires.Application.Cagnottes.Queries.GetAllCagnottes;
using GestionPrestataires.Application.Cagnottes.Queries.GetCagnotteActuelle;
using GestionPrestataires.Application.Cagnottes.Queries.GetCagnotteByAnnee;
using GestionPrestataires.Application.Cagnottes.Queries.GetSourcesByCagnotteId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionPrestataires.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]  //   ADMIN UNIQUEMENT
public class CagnottesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CagnottesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _mediator.Send(new GetAllCagnottesQuery());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur" });
        }
    }

    [HttpGet("actuelle")]
    public async Task<IActionResult> GetActuelle()
    {
        try
        {
            var result = await _mediator.Send(new GetCagnotteActuelleQuery());
            if (result == null)
                return NotFound(new { message = "Aucune cagnotte définie pour l'année en cours" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur" });
        }
    }

    [HttpGet("annee/{annee}")]
    public async Task<IActionResult> GetByAnnee(int annee)
    {
        var result = await _mediator.Send(new GetCagnotteByAnneeQuery(annee));
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCagnotteDto dto)
    {
        try
        {
            var command = new CreateCagnotteCommand(dto.Annee, 0);
            var cagnotte = await _mediator.Send(command);

            if (dto.SourcesBudget != null && dto.SourcesBudget.Any())
            {
                foreach (var source in dto.SourcesBudget)
                {
                    await _mediator.Send(new AddSourceBudgetCommand(
                        cagnotte.Id,
                        source.Nom,
                        source.Code,
                        source.Montant,
                        source.Description
                    ));
                }
            }

            return CreatedAtAction(nameof(GetByAnnee), new { annee = cagnotte.Annee }, cagnotte);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCagnotteDto dto)
    {
        try
        {
            var sourcesActuelles = await _mediator.Send(new GetSourcesByCagnotteIdQuery(id));

            foreach (var sourceActuelle in sourcesActuelles)
            {
                if (!dto.SourcesBudget.Any(s => s.Id == sourceActuelle.Id.ToString()))
                {
                    await _mediator.Send(new DeleteSourceBudgetCommand(sourceActuelle.Id));
                }
            }

            foreach (var sourceDto in dto.SourcesBudget)
            {
                if (string.IsNullOrEmpty(sourceDto.Id))
                {
                    await _mediator.Send(new AddSourceBudgetCommand(
                        id,
                        sourceDto.Nom,
                        sourceDto.Code,
                        sourceDto.Montant,
                        sourceDto.Description
                    ));
                }
                else
                {
                    await _mediator.Send(new UpdateSourceBudgetCommand(
                        Guid.Parse(sourceDto.Id),
                        sourceDto.Nom,
                        sourceDto.Code,
                        sourceDto.Montant,
                        sourceDto.Description
                    ));
                }
            }

            var command = new UpdateCagnotteCommand(id);
            await _mediator.Send(command);

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("recalculer/{annee}")]
    public async Task<IActionResult> Recalculer(int annee)
    {
        try
        {
            await _mediator.Send(new RecalculerBudgetCommand(annee));
            return Ok(new { message = $"Budget recalculé pour l'année {annee}" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Erreur serveur" });
        }
    }

    [HttpGet("{cagnotteId}/sources")]
    public async Task<IActionResult> GetSourcesBudget(Guid cagnotteId)
    {
        try
        {
            var sources = await _mediator.Send(new GetSourcesByCagnotteIdQuery(cagnotteId));
            return Ok(sources);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{cagnotteId}/sources")]
    public async Task<IActionResult> AddSourceBudget(Guid cagnotteId, [FromBody] SourceBudgetDto dto)
    {
        try
        {
            var command = new AddSourceBudgetCommand(
                cagnotteId,
                dto.Nom,
                dto.Code,
                dto.Montant,
                dto.Description
            );
            var source = await _mediator.Send(command);
            return Ok(source);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("sources/{sourceId}")]
    public async Task<IActionResult> UpdateSourceBudget(Guid sourceId, [FromBody] SourceBudgetDto dto)
    {
        try
        {
            var command = new UpdateSourceBudgetCommand(
                sourceId,
                dto.Nom,
                dto.Code,
                dto.Montant,
                dto.Description
            );
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("sources/{sourceId}")]
    public async Task<IActionResult> DeleteSourceBudget(Guid sourceId)
    {
        try
        {
            await _mediator.Send(new DeleteSourceBudgetCommand(sourceId));
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteCagnotteCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}

// DTOs
public record CreateCagnotteDto
{
    public int Annee { get; init; }
    public List<SourceBudgetDto>? SourcesBudget { get; init; }
}

public record UpdateCagnotteDto
{
    public List<SourceBudgetDtoWithId> SourcesBudget { get; init; } = new();
}

public record SourceBudgetDto
{
    public string Nom { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public decimal Montant { get; init; }
    public string? Description { get; init; }
}

public record SourceBudgetDtoWithId
{
    public string? Id { get; init; }
    public string Nom { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public decimal Montant { get; init; }
    public string? Description { get; init; }
}