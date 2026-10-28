using GestionPrestataires.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.TypeShifts.Queries
{
 
    public class GetTypeShiftByIdQuery : IRequest<TypeShift>
    {
        public Guid Id { get; set; }
     }
}
