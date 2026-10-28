using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories;

public interface ITypeContratRepository
{
    Task<IEnumerable<TypeContrat>> GetAllAsync();
    Task<TypeContrat?> GetByIdAsync(Guid id);
    Task<TypeContrat> AddAsync(TypeContrat typeContrat);
    Task UpdateAsync(TypeContrat typeContrat);
    Task DeleteAsync(Guid id);
}
