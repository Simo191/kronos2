using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.TypeShifts.Commands
{
    public class DeleteTypeShiftCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
