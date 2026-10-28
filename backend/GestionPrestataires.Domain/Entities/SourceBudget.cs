namespace GestionPrestataires.Domain.Entities;

public class SourceBudget
{
    public Guid Id { get; set; }
    public Guid CagnotteId { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // Ex: "CAPEX", "LEA LICENCES", "UDI"
    public decimal Montant { get; set; }
    public string? Description { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime DateModification { get; set; }

    // Navigation
    public Cagnotte? Cagnotte { get; set; }
}