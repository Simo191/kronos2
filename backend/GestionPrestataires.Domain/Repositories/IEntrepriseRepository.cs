using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories;

public interface IEntrepriseRepository
{
    Task<IEnumerable<Entreprise>> GetAllAsync(bool includeInactive = false);
    Task<Entreprise?> GetByIdAsync(Guid id);
    Task<Entreprise> AddAsync(Entreprise entreprise);
    Task UpdateAsync(Entreprise entreprise);
    Task DeleteAsync(Guid id);
}
