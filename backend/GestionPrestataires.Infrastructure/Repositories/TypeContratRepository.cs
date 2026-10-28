using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Repositories;

public class TypeContratRepository : ITypeContratRepository
{
    private readonly AppDbContext _context;

    public TypeContratRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TypeContrat>> GetAllAsync()
    {
        return await _context.TypesContrat
            .OrderBy(t => t.Libelle)
            .ToListAsync();
    }

    public async Task<TypeContrat?> GetByIdAsync(Guid id)
    {
        return await _context.TypesContrat.FindAsync(id);
    }

    public async Task<TypeContrat> AddAsync(TypeContrat typeContrat)
    {
        _context.TypesContrat.Add(typeContrat);
        await _context.SaveChangesAsync();
        return typeContrat;
    }

    public async Task UpdateAsync(TypeContrat typeContrat)
    {
        _context.Entry(typeContrat).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var typeContrat = await _context.TypesContrat.FindAsync(id);
        if (typeContrat != null)
        {
            _context.TypesContrat.Remove(typeContrat);
            await _context.SaveChangesAsync();
        }
    }
}
