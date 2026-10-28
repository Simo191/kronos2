using GestionPrestataires.Application.DTOs;
using MediatR;

namespace GestionPrestataires.Application.Shifts.Queries
{
    public class GetShiftsByMonthQuery : IRequest<List<ShiftDTO>>
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public Guid? PrestataireId { get; set; }
    }
}
