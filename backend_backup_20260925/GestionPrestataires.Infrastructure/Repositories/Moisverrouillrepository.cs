using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Persistence.Repositories;

public class MoisVerrouillRepository : IMoisVerrouillRepository
{
    private readonly AppDbContext _context;

    public MoisVerrouillRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MoisVerrouille>> GetAllAsync()
    {
        return await _context.MoisVerrouilles
            .Include(m => m.VerrouilleParUser)
            .OrderByDescending(m => m.Mois)
            .ToListAsync();
    }

    public async Task<MoisVerrouille?> GetByMoisAsync(string mois)
    {
        return await _context.MoisVerrouilles
            .Include(m => m.VerrouilleParUser)
            .FirstOrDefaultAsync(m => m.Mois == mois);
    }

    public async Task<bool> IsMoisVerrouille(string mois)
    {
        return await _context.MoisVerrouilles
            .AnyAsync(m => m.Mois == mois);
    }

    public async Task AddAsync(MoisVerrouille moisVerrouille)
    {
        _context.MoisVerrouilles.Add(moisVerrouille);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var moisVerrouille = await _context.MoisVerrouilles.FindAsync(id);
        if (moisVerrouille != null)
        {
            _context.MoisVerrouilles.Remove(moisVerrouille);
            await _context.SaveChangesAsync();
        }
    }
}
