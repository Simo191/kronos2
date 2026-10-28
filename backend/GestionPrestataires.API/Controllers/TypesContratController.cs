using GestionPrestataires.Application.TypesContrat.Commands.CreateTypeContrat;
using GestionPrestataires.Application.TypesContrat.Commands.DeleteTypeContrat;
using GestionPrestataires.Application.TypesContrat.Commands.UpdateTypeContrat;
using GestionPrestataires.Application.TypesContrat.Queries.GetAllTypesContrat;
using GestionPrestataires.Application.TypesContrat.Queries.GetTypeContratById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionPrestataires.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]  //   ADMIN UNIQUEMENT
public class TypesContratController : ControllerBase
{
    private readonly IMediator _mediator;

    public TypesContratController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllTypesContratQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetTypeContratByIdQuery(id));
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTypeContratCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erreur création", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTypeContratCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteTypeContratCommand(id));
        return NoContent();
    }
}