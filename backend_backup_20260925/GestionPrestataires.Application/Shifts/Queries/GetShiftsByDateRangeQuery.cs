using GestionPrestataires.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.Shifts.Queries
{
    public class GetShiftsByDateRangeQuery : IRequest<List<ShiftDTO>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? PrestataireId { get; set; }
    }
}
