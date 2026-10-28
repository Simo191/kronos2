using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.DTOs;

public class ShiftDTO
{
    public Guid Id { get; set; }
    public Guid PrestataireId { get; set; }
    public string PrestataireNom { get; set; }
    public string PrestatairePrenom { get; set; }
    public DateTime Date { get; set; }
    public Guid TypeShiftId { get; set; }
    public TypeShiftDTO TypeShift { get; set; }
    public string Notes { get; set; }
    public decimal? NombreJours { get; set; }

}

public class TypeShiftDTO
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string ShortLabel { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string BgColor { get; set; } = string.Empty;
    public string BorderColor { get; set; } = string.Empty;
    public string TextColor { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Order { get; set; }
    public ShiftCategory Category { get; set; } //   NOUVEAU
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

