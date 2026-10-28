using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories;

public interface IPrestataireCredentialRepository
{
    Task<IEnumerable<PrestataireCredential>> GetByPrestataireIdAsync(Guid prestataireId);
    Task<PrestataireCredential?> GetByIdAsync(Guid id);
    Task<PrestataireCredential> AddAsync(PrestataireCredential credential);
    Task UpdateAsync(PrestataireCredential credential);
    Task DeleteAsync(Guid id);
}
