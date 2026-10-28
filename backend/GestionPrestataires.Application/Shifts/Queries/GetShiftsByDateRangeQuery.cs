using GestionPrestataires.Application.DTOs;
using MediatR;

namespace GestionPrestataires.Application.Shifts.Queries
{
    public class GetShiftsByDateRangeQuery : IRequest<List<ShiftDTO>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? PrestataireId { get; set; }
    }
}
