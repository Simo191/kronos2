using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories;

public interface IContratRepository
{
    IQueryable<Contrat> GetAllAsync();
    Task<Contrat?> GetByIdAsync(Guid id);
    Task<Contrat> AddAsync(Contrat contrat);
    Task UpdateAsync(Contrat contrat);
    Task DeleteAsync(Guid id);
    IQueryable<Contrat> GetByPrestataireIdAsync(Guid PrestataireId);
}
