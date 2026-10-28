using GestionPrestataires.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.TypeShifts.Commands
{
    public class DeleteTypeShiftCommandHandler : IRequestHandler<DeleteTypeShiftCommand, Unit>
    {
        private readonly ITypeShiftRepository _context;

        public DeleteTypeShiftCommandHandler(ITypeShiftRepository context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteTypeShiftCommand request, CancellationToken cancellationToken)
        {
            var typeShift = await _context.GetByIdAsync(request.Id);

            if (typeShift == null)
            {
                throw new Exception($"TypeShift {request.Id} not found");
            }

            await _context.DeleteAsync(typeShift);
 
            return Unit.Value;
        }
    }
}
