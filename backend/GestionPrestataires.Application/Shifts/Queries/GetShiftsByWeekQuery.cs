using GestionPrestataires.Application.DTOs;
using MediatR;

namespace GestionPrestataires.Application.Shifts.Queries
{
    public class GetShiftsByWeekQuery : IRequest<List<ShiftDTO>>
    {
        public DateTime WeekStartDate { get; set; }
        public Guid? PrestataireId { get; set; }
    }
}
