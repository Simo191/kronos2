using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories;

public interface ISourceBudgetRepository
{
    Task<SourceBudget?> GetByIdAsync(Guid id);
    Task<List<SourceBudget>> GetByCagnotteIdAsync(Guid cagnotteId);
    Task<SourceBudget> AddAsync(SourceBudget sourceBudget);
    Task UpdateAsync(SourceBudget sourceBudget);
    Task DeleteAsync(Guid id);
}