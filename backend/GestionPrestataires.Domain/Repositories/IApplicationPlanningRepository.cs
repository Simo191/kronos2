using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories;

public interface IApplicationPlanningRepository
{
    Task<IEnumerable<ApplicationPlanning>> GetAllAsync();
    Task<ApplicationPlanning?> GetByIdAsync(Guid id);
    Task<ApplicationPlanning> AddAsync(ApplicationPlanning app);
    Task UpdateAsync(ApplicationPlanning app);
    Task DeleteAsync(Guid id);
}