using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories;

public interface IEstimationRepository
{
    Task<IEnumerable<Estimation>> GetAllAsync();
    Task<Estimation?> GetByIdAsync(Guid id);
    Task<Estimation?> GetByIdWithLignesAsync(Guid id);
    Task<Estimation?> GetByAnneeAsync(int annee);
    Task AddAsync(Estimation estimation);
    Task UpdateAsync(Estimation estimation);
    Task DeleteAsync(Guid id);
    Task<Estimation?> GetByIdWithLignesNotrackAsync(Guid id);
}