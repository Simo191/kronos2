using GestionPrestataires.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.Shifts.Commands
{
    public class CreateShiftCommandHandler : IRequestHandler<CreateShiftCommand, Guid>
    {
        private readonly IShiftRepository _context;

        public CreateShiftCommandHandler(IShiftRepository context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateShiftCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var shift = new Shift
                {
                    Id = Guid.NewGuid(),
                    PrestataireId = request.PrestataireId,
                    Date = request.Date,
                    TypeShiftId = request.TypeShiftId,
                    Notes = request.Notes,
                    NombreJours = request.NombreJours,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.AddAsync(shift);
                return shift.Id;


            }
            catch (Exception ex )
            {

                throw;
            }
            

 
        }
    }
}
