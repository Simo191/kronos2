using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories;

public interface ICagnotteRepository
{
    Task<List<Cagnotte>> GetAllAsync();
    Task<Cagnotte?> GetByIdAsync(Guid id);
    Task<Cagnotte?> GetByAnneeAsync(int annee);
    Task<Cagnotte> AddAsync(Cagnotte cagnotte);
    Task UpdateAsync(Cagnotte cagnotte);
    Task DeleteAsync(Guid id);
    Task<decimal> CalculerBudgetConsommeAsync(int Annee);
}
