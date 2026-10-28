using MediatR;

namespace GestionPrestataires.Application.Shifts.Commands
{
    public class CreateShiftCommand : IRequest<Guid>
    {
        public Guid PrestataireId { get; set; }
        public DateTime Date { get; set; }
        public Guid TypeShiftId { get; set; }
        public string? Notes { get; set; }
        public decimal NombreJours { get; set; }
    }
}
