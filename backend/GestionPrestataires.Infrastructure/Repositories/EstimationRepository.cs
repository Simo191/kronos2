using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Persistence.Repositories;

public class EstimationRepository : IEstimationRepository
{
    private readonly AppDbContext _context;

    public EstimationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Estimation>> GetAllAsync()
    {
        return await _context.Estimations
            .Include(e => e.Createur)
            .Include(e => e.Modificateur)
            .Include(e => e.Lignes)
            .OrderByDescending(e => e.Annee)
            .ToListAsync();
    }

    public async Task<Estimation?> GetByIdAsync(Guid id)
    {
        return await _context.Estimations
            .Include(e => e.Createur)
            .Include(e => e.Modificateur)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Estimation?> GetByIdWithLignesAsync(Guid id)
    {
        return await _context.Estimations
            .Include(e => e.Createur)
            .Include(e => e.Modificateur)
            .Include(e => e.Lignes)
                .ThenInclude(l => l.Prestataire)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    public async Task<Estimation?> GetByIdWithLignesNotrackAsync(Guid id)
    {
        return await _context.Estimations
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    public async Task<Estimation?> GetByAnneeAsync(int annee)
    {
        return await _context.Estimations
            .Include(e => e.Createur)
            .Include(e => e.Modificateur)
            .Include(e => e.Lignes)
                .ThenInclude(l => l.Prestataire)
            .FirstOrDefaultAsync(e => e.Annee == annee);
    }

    public async Task AddAsync(Estimation estimation)
    {
        _context.Estimations.Add(estimation);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Estimation estimation)
    {
        _context.Estimations.Update(estimation);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var estimation = await _context.Estimations
            .Include(e => e.Lignes)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (estimation != null)
        {
            _context.Estimations.Remove(estimation);
            await _context.SaveChangesAsync();
        }
    }
}