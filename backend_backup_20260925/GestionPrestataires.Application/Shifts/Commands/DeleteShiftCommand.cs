using GestionPrestataires.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.Shifts.Commands
{
    public class DeleteShiftCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
