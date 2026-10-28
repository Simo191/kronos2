using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Repositories;

public class EntrepriseRepository : IEntrepriseRepository
{
    private readonly AppDbContext _context;

    public EntrepriseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Entreprise>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Entreprises.AsQueryable();
        if (!includeInactive) query = query.Where(e => e.IsActive);
        return await query.OrderBy(e => e.Nom).ToListAsync();
    }

    public async Task<Entreprise?> GetByIdAsync(Guid id)
        => await _context.Entreprises.FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Entreprise> AddAsync(Entreprise e)
    {
        _context.Entreprises.Add(e);
        await _context.SaveChangesAsync();
        return e;
    }

    public async Task UpdateAsync(Entreprise e)
    {
        _context.Entry(e).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var e = await _context.Entreprises.FirstOrDefaultAsync(x => x.Id == id);
        if (e != null)
        {
            _context.Entreprises.Remove(e);
            await _context.SaveChangesAsync();
        }
    }
}
