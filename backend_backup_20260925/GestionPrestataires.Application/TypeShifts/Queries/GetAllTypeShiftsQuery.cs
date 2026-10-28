using GestionPrestataires.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.TypeShifts.Queries
{
    public class GetAllTypeShiftsQuery : IRequest<List<TypeShift>>
    {
        public bool ActiveOnly { get; set; } = true;
    }
}
