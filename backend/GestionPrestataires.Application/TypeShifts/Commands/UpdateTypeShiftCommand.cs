using MediatR;

namespace GestionPrestataires.Application.TypeShifts.Commands
{
    public class UpdateTypeShiftCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string? Label { get; set; }
        public string? ShortLabel { get; set; }
        public string? Code { get; set; }
        public string? Color { get; set; }
        public string? BgColor { get; set; }
        public string? BorderColor { get; set; }
        public string? TextColor { get; set; }
        public int? Order { get; set; }
        public ShiftCategory? Category { get; set; } //   NOUVEAU
        public bool? IsActive { get; set; }
    }
}
