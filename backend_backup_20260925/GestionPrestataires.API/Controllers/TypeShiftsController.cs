using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Application.TypeShifts.Commands;
using GestionPrestataires.Application.TypeShifts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionPrestataires.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager,User")]  //   ADMIN UNIQUEMENT
    public class TypeShiftsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TypeShiftsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Récupérer tous les types de shifts
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TypeShiftDTO>>> GetAll([FromQuery] bool onlyActive = false)
        {
            var query = new GetAllTypeShiftsQuery { ActiveOnly = onlyActive };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Récupérer un type de shift par ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TypeShiftDTO>> GetById(Guid id)
        {
            var query = new GetTypeShiftByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { message = $"TypeShift avec l'ID {id} n'a pas été trouvé." });

            return Ok(result);
        }

        /// <summary>
        /// Créer un nouveau type de shift
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateTypeShiftRequest request)
        {
            var command = new CreateTypeShiftCommand
            {
                Label = request.Label,
                ShortLabel = request.ShortLabel,
                Code = request.Code,
                Color = request.Color,
                BgColor = request.BgColor,
                BorderColor = request.BorderColor,
                TextColor = request.TextColor,
                Order = request.Order,
                Category = request.Category,
                IsActive = request.IsActive
            };

            try
            {
                var id = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Mettre à jour un type de shift
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateTypeShiftRequest request)
        {
            var command = new UpdateTypeShiftCommand
            {
                Id = id,
                Label = request.Label,
                ShortLabel = request.ShortLabel,
                Code = request.Code,
                Color = request.Color,
                BgColor = request.BgColor,
                BorderColor = request.BorderColor,
                TextColor = request.TextColor,
                Order = request.Order,
                Category = request.Category,
                IsActive = request.IsActive
            };

            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Supprimer un type de shift
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var command = new DeleteTypeShiftCommand { Id = id };

            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }

    // Request DTOs
    public class CreateTypeShiftRequest
    {
        public string Label { get; set; } = string.Empty;
        public string ShortLabel { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string BgColor { get; set; } = string.Empty;
        public string BorderColor { get; set; } = string.Empty;
        public string TextColor { get; set; } = string.Empty;
        public int Order { get; set; }
        public ShiftCategory Category { get; set; } = ShiftCategory.Travail;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateTypeShiftRequest
    {
        public string? Label { get; set; }
        public string? ShortLabel { get; set; }
        public string? Code { get; set; }
        public string? Color { get; set; }
        public string? BgColor { get; set; }
        public string? BorderColor { get; set; }
        public string? TextColor { get; set; }
        public int? Order { get; set; }
        public ShiftCategory? Category { get; set; }
        public bool? IsActive { get; set; }
    }
}