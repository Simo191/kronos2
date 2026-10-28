using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories;

public interface IJourTravailRepository
{
    Task<IEnumerable<JourTravail>> GetAllAsync(Guid? prestataireId = null, string? mois = null);
    Task<JourTravail?> GetByIdAsync(Guid id);
    Task<JourTravail> AddAsync(JourTravail jourTravail);
    Task UpdateAsync(JourTravail jourTravail);
    Task DeleteAsync(Guid id);
    Task<decimal> GetbypresttaireAndContratAllAsync(Guid? prestataireId, DateTime DateDebut, DateTime DateFin);
}
