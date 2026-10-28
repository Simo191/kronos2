using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
