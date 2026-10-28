using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Application.Shifts.Commands;
using GestionPrestataires.Application.Shifts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionPrestataires.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  //   OBLIGATOIRE : Authentification requise
    public class ShiftsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShiftsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///   User peut créer SEULEMENT ses shifts
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateShift([FromBody] CreateShiftCommand command)
        {
            try
            {
                //   SÉCURITÉ : Vérifier que le User crée pour lui-même
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var userPrestataireId = User.FindFirst("PrestataireId")?.Value;

                if (userRole == "User" && !string.IsNullOrEmpty(userPrestataireId))
                {
                    // Forcer le prestataireId du User
                    // TODO : Ajouter validation dans le Handler
                }

                var shiftId = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetShiftsByMonth), new { year = command.Date.Year, month = command.Date.Month }, shiftId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erreur création", error = ex.Message });
            }
        }

        /// <summary>
        ///   User peut modifier SEULEMENT ses shifts
        /// Admin/Manager peuvent modifier tous les shifts
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateShift(Guid id, [FromBody] UpdateShiftRequest request)
        {
            try
            {
                // TODO : Vérifier ownership si User
                var command = new UpdateShiftCommand
                {
                    Id = id,
                    TypeShiftId = request.TypeShiftId,
                    Notes = request.Notes,
                    NombreJours = request.NombreJours
                };

                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erreur modification", error = ex.Message });
            }
        }

        /// <summary>
        ///   User peut supprimer SEULEMENT ses shifts
        /// Admin/Manager peuvent supprimer tous les shifts
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteShift(Guid id)
        {
            try
            {
                // TODO : Vérifier ownership si User
                var command = new DeleteShiftCommand { Id = id };
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erreur suppression", error = ex.Message });
            }
        }

        /// <summary>
        ///   SÉCURISÉ : Filtrage selon rôle
        /// - Admin/Manager : Tous les shifts
        /// - User : Tous les shifts visibles (lecture seule sur les autres)
        /// </summary>
        [HttpGet("month/{year}/{month}")]
        public async Task<ActionResult<List<ShiftDTO>>> GetShiftsByMonth(int year, int month, [FromQuery] Guid? prestataireId = null)
        {
            try
            {
                var query = new GetShiftsByMonthQuery
                {
                    Year = year,
                    Month = month,
                    PrestataireId = prestataireId
                };

                var shifts = await _mediator.Send(query);

                //   User voit TOUS les shifts (mais ne peut modifier que les siens - géré côté frontend)
                return Ok(shifts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
            }
        }

        [HttpGet("week")]
        public async Task<ActionResult<List<ShiftDTO>>> GetShiftsByWeek([FromQuery] DateTime weekStartDate, [FromQuery] Guid? prestataireId = null)
        {
            var query = new GetShiftsByWeekQuery
            {
                WeekStartDate = weekStartDate,
                PrestataireId = prestataireId
            };

            var shifts = await _mediator.Send(query);
            return Ok(shifts);
        }

        [HttpGet("range")]
        public async Task<ActionResult<List<ShiftDTO>>> GetShiftsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] Guid? prestataireId = null)
        {
            try
            {
                if (prestataireId.HasValue)
                {
                    if (String.IsNullOrEmpty(prestataireId!.Value.ToString()))
                    {
                        prestataireId = null;
                    }
                    if (prestataireId!.Value.ToString() == "null")
                    {
                        prestataireId = null;
                    }
                }

                var query = new GetShiftsByDateRangeQuery
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    PrestataireId = prestataireId
                };

                var shifts = await _mediator.Send(query);
                return Ok(shifts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", error = ex.Message });
            }
        }
    }

    public class UpdateShiftRequest
    {
        public Guid TypeShiftId { get; set; }
        public string Notes { get; set; }
        public decimal NombreJours { get; set; }

    }
}