using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories;

public interface IPrestataireRepository
{
    Task<IEnumerable<Prestataire>> GetAllAsync(bool includeInactive = false);
    Task<Prestataire?> GetByIdAsync(Guid id);
    Task<Prestataire> AddAsync(Prestataire prestataire);
    Task UpdateAsync(Prestataire prestataire);
    Task DeleteAsync(Guid id);
    Task<Prestataire?> GetByIdUserAsync(Guid id);
}
