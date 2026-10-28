namespace GestionPrestataires.Domain.Entities;

//   Jours prévus d'un contrat sur une année civile (ventilation budgétaire).
//   Le montant de l'année est calculé : MontantTotal × NombreJours / Contrat.NombreJours.
public class ContratRepartitionAnnuelle
{
    public Guid Id { get; set; }
    public Guid ContratId { get; set; }
    public int Annee { get; set; }
    public decimal NombreJours { get; set; }

    public Contrat? Contrat { get; set; }
}
