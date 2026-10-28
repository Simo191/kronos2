using GestionPrestataires.Domain.Entities;
public enum ShiftCategory
{
    Travail = 0,      // Sur site, télétravail, etc.
    Absence = 1,      // Absence
    Conge = 2         // Congés
}
public class Shift
{
    public Guid Id { get; set; }

    public Guid PrestataireId { get; set; }
    public Prestataire Prestataire { get; set; } = default!; //   navigation liée

    public DateTime Date { get; set; }

    public Guid TypeShiftId { get; set; }
    public TypeShift TypeShift { get; set; } = default!;
    public decimal? NombreJours { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


public class TypeShift
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string ShortLabel { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string BgColor { get; set; } = string.Empty;
    public string BorderColor { get; set; } = string.Empty;
    public string TextColor { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int Order { get; set; }
    public ShiftCategory Category { get; set; } = ShiftCategory.Travail;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}