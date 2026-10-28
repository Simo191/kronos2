using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories
{

    public interface IApplicationPrestataireAssignmentRepository
    {
        Task<IEnumerable<ApplicationPrestataireAssignment>> GetAllAsync();
        Task<ApplicationPrestataireAssignment?> GetByIdAsync(Guid id);
        Task<ApplicationPrestataireAssignment> AddAsync(ApplicationPrestataireAssignment app);
        Task UpdateAsync(ApplicationPrestataireAssignment app);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<ApplicationPrestataireAssignment>> GetIdApplicationAsync(Guid ApplicationId);
        Task DeleteAllAsync(Guid ApplicationId);

        Task AddRangeAsync(IEnumerable<ApplicationPrestataireAssignment> entities);
    }
}
