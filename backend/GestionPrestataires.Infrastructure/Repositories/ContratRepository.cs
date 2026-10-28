using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Repositories;

public class ContratRepository : IContratRepository
{
    private readonly AppDbContext _context;

    public ContratRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Contrat> GetAllAsync()
    {
        return _context.Contrats
            .Include(c => c.Prestataire)
                .ThenInclude(p => p!.Entreprise)
            .Include(c => c.TypeContrat)
            .Include(c => c.Application)
            .Include(c => c.Repartitions)
            .OrderByDescending(c => c.DateDebut)
            .AsQueryable();
    }
    public IQueryable<Contrat> GetByPrestataireIdAsync(Guid PrestataireId)
    {
        return _context.Contrats
            .Include(c => c.Prestataire)
                .ThenInclude(p => p!.Entreprise)
            .Include(c => c.TypeContrat)
            .OrderByDescending(c => c.DateDebut).Where(w => w.PrestataireId == PrestataireId)
            .AsQueryable();
    }
    public async Task<Contrat?> GetByIdAsync(Guid id)
    {
        return await _context.Contrats
            .Include(c => c.Prestataire)
                .ThenInclude(p => p!.Entreprise)
            .Include(c => c.TypeContrat)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    //   Remplace la répartition annuelle d'un contrat (suppression puis insertion).
    //   Les lignes ne sont pas chargées par GetByIdAsync : pas de conflit de tracking EF.
    public async Task ReplaceRepartitionsAsync(Guid contratId, IEnumerable<ContratRepartitionAnnuelle> repartitions)
    {
        var existantes = await _context.ContratRepartitions
            .Where(r => r.ContratId == contratId)
            .ToListAsync();
        _context.ContratRepartitions.RemoveRange(existantes);

        foreach (var r in repartitions)
        {
            r.ContratId = contratId;
            if (r.Id == Guid.Empty) r.Id = Guid.NewGuid();
            _context.ContratRepartitions.Add(r);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Contrat> AddAsync(Contrat contrat)
    {
        _context.Contrats.Add(contrat);
        await _context.SaveChangesAsync();
        return contrat;
    }

    public async Task UpdateAsync(Contrat contrat)
    {
        _context.Entry(contrat).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var contrat = await _context.Contrats.FindAsync(id);
        if (contrat != null)
        {
            _context.Contrats.Remove(contrat);
            await _context.SaveChangesAsync();
        }
    }
}
