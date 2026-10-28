using GestionPrestataires.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.Shifts.Commands
{
    public class DeleteShiftCommandHandler : IRequestHandler<DeleteShiftCommand, Unit>
    {
        private readonly IShiftRepository _context;

        public DeleteShiftCommandHandler(IShiftRepository context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteShiftCommand request, CancellationToken cancellationToken)
        {
            var shift = await _context.GetByIdAsync(request.Id);

            if (shift == null)
            {
                throw new Exception($"Shift {request.Id} not found");
            }

           await _context.DeleteAsync(shift);
 
            return Unit.Value;
        }
    }
}
