using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
    Task<List<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role> AddAsync(Role prestataire);
    Task UpdateAsync(Role prestataire);
    Task DeleteAsync(Guid id);
}
