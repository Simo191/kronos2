using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using System.Threading;

namespace GestionPrestataires.Infrastructure.Repositories;

public class JourTravailRepository : IJourTravailRepository
{
    private readonly AppDbContext _context;

    public JourTravailRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<JourTravail>> GetAllAsync(Guid? prestataireId = null, string? mois = null)
    {
        var query = _context.JoursTravail
            .Include(j => j.Prestataire)
            .AsQueryable();

        if (prestataireId.HasValue)
            query = query.Where(j => j.PrestataireId == prestataireId.Value);



        if (mois != null)
            query = query.Where(j => j.Mois == mois);

        return await query
            .OrderByDescending(j => j.Mois)
            .ToListAsync();
    }

    public async Task<JourTravail?> GetByIdAsync(Guid id)
    {
        return await _context.JoursTravail
            .Include(j => j.Prestataire)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<JourTravail> AddAsync(JourTravail jourTravail)
    {
        _context.JoursTravail.Add(jourTravail);
        await _context.SaveChangesAsync();
        return jourTravail;
    }

    public async Task UpdateAsync(JourTravail jourTravail)
    {
        _context.Entry(jourTravail).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var jourTravail = await _context.JoursTravail.FindAsync(id);
        if (jourTravail != null)
        {
            _context.JoursTravail.Remove(jourTravail);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<decimal> GetbypresttaireAndContratAllAsync(Guid? prestataireId,DateTime DateDebut, DateTime DateFin)
    {
        return await _context.JoursTravail.Where(jt => jt.PrestataireId == prestataireId
                      && jt.DateTravail >= DateDebut
                      && jt.DateTravail <= DateFin)
            .SumAsync(jt => jt.NombreJours); // SUM pas COUNT !            .AsQueryable();
    }
}
