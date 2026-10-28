using MediatR;

namespace GestionPrestataires.Application.Shifts.Commands
{
    public class DeleteShiftCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
