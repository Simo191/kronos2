using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories
{
    public interface ILigneEstimationRepository : IRepository<LigneEstimation>
    {
        Task<List<LigneEstimation>> GetByIdWithLignesAsync(Guid id);
    }
}
