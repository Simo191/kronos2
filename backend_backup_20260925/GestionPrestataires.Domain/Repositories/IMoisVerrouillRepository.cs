using GestionPrestataires.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Domain.Repositories;

public interface IMoisVerrouillRepository
{
    Task<IEnumerable<MoisVerrouille>> GetAllAsync();
    Task<MoisVerrouille?> GetByMoisAsync(string mois);
    Task<bool> IsMoisVerrouille(string mois);
    Task AddAsync(MoisVerrouille moisVerrouille);
    Task DeleteAsync(Guid id);
}
