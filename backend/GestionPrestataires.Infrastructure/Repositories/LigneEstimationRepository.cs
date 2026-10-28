using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Repositories
{
    public class LigneEstimationRepository : Repository<LigneEstimation>, ILigneEstimationRepository
    {
        public LigneEstimationRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<LigneEstimation>> GetByIdWithLignesAsync(Guid id)
        {
            return await _context.LigneEstimations.Where(w => w.EstimationId == id).ToListAsync();
        }
    }
}
