using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            .Include(c => c.TypeContrat)
            .OrderByDescending(c => c.DateDebut)
            .AsQueryable();
    }
    public IQueryable<Contrat> GetByPrestataireIdAsync(Guid PrestataireId)
    {
        return _context.Contrats
            .Include(c => c.Prestataire)
            .Include(c => c.TypeContrat)
            .OrderByDescending(c => c.DateDebut).Where(w=>w.PrestataireId == PrestataireId)
            .AsQueryable();
    }
    public async Task<Contrat?> GetByIdAsync(Guid id)
    {
        return await _context.Contrats
            .Include(c => c.Prestataire)
            .Include(c => c.TypeContrat)
            .FirstOrDefaultAsync(c => c.Id == id);
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
