using MediatR;

namespace GestionPrestataires.Application.TypeShifts.Commands
{
    public class DeleteTypeShiftCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
