using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.Shifts.Commands
{
    public class UpdateShiftCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public Guid TypeShiftId { get; set; }
        public string Notes { get; set; }
        public decimal NombreJours { get; set; }

    }
}
