using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Shifts.Commands
{
    public class UpdateShiftCommandHandler : IRequestHandler<UpdateShiftCommand, Unit>
    {
        private readonly IShiftRepository _context;

        public UpdateShiftCommandHandler(IShiftRepository context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateShiftCommand request, CancellationToken cancellationToken)
        {
            var shift = await _context.GetByIdAsync(request.Id);

            if (shift == null)
            {
                throw new Exception($"Shift {request.Id} not found");
            }

            shift.TypeShiftId = request.TypeShiftId;
            shift.Notes = request.Notes;
            shift.NombreJours = request.NombreJours;
            shift.UpdatedAt = DateTime.UtcNow;

            await _context.UpdateAsync(shift);

            return Unit.Value;
        }
    }
}
