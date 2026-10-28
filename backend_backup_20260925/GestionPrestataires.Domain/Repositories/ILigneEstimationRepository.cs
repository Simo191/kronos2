using GestionPrestataires.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Domain.Repositories
{
    public interface ILigneEstimationRepository : IRepository<LigneEstimation>
    {
        Task<List<LigneEstimation>> GetByIdWithLignesAsync(Guid id);
    }
}
