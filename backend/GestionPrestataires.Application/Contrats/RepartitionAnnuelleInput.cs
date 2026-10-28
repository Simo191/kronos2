using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Application.Contrats;

public record RepartitionAnnuelleInput(int Annee, decimal NombreJours)
{
    //   Une ligne par année, jours > 0 uniquement
    public static List<ContratRepartitionAnnuelle> VersEntites(IEnumerable<RepartitionAnnuelleInput> lignes) =>
        lignes.Where(l => l.NombreJours > 0)
              .GroupBy(l => l.Annee)
              .Select(g => new ContratRepartitionAnnuelle
              {
                  Id = Guid.NewGuid(),
                  Annee = g.Key,
                  NombreJours = g.Sum(l => l.NombreJours)
              })
              .ToList();
}
