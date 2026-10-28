using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Repositories;

public class PrestataireRepository : IPrestataireRepository
{
    private readonly AppDbContext _context;

    public PrestataireRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Prestataire>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Prestataires
            .Include(p => p.Entreprise)
            .AsQueryable();
        if (!includeInactive)
            query = query.Where(p => p.IsActive);

        return await query
            .OrderBy(p => p.Order)
            .ToListAsync();
    }

    public async Task<Prestataire?> GetByIdAsync(Guid id)
    {
        return await _context.Prestataires
            .Include(p => p.Entreprise)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task<Prestataire?> GetByIdUserAsync(Guid id)
    {
        return await _context.Prestataires.FirstOrDefaultAsync(f => f.UserId == id);
    }

    public async Task<Prestataire> AddAsync(Prestataire prestataire)
    {
        _context.Prestataires.Add(prestataire);
        await _context.SaveChangesAsync();
        return prestataire;
    }

    public async Task UpdateAsync(Prestataire prestataire)
    {
        _context.Entry(prestataire).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var prestataire = await _context.Prestataires.FindAsync(id);
        if (prestataire != null)
        {
            _context.Prestataires.Remove(prestataire);
            await _context.SaveChangesAsync();
        }
    }
}
