using GestionPrestataires.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.Shifts.Queries
{
    public class GetShiftsByMonthQuery : IRequest<List<ShiftDTO>>
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public Guid? PrestataireId { get; set; }
    }
}
