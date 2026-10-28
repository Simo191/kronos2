using MediatR;

namespace GestionPrestataires.Application.TypeShifts.Commands
{
    public class CreateTypeShiftCommand : IRequest<Guid>
    {
        public string Label { get; set; } = string.Empty;
        public string ShortLabel { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string BgColor { get; set; } = string.Empty;
        public string BorderColor { get; set; } = string.Empty;
        public string TextColor { get; set; } = string.Empty;
        public int Order { get; set; }
        public ShiftCategory Category { get; set; } = ShiftCategory.Travail; //   NOUVEAU
        public bool IsActive { get; set; } = true;
    }
}
