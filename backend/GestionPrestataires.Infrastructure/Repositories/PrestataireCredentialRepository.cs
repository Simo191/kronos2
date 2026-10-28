using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Repositories;

public class PrestataireCredentialRepository : IPrestataireCredentialRepository
{
    private readonly AppDbContext _context;

    public PrestataireCredentialRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PrestataireCredential>> GetByPrestataireIdAsync(Guid prestataireId)
    {
        return await _context.PrestataireCredentials
            .Where(c => c.PrestataireId == prestataireId)
            .OrderBy(c => c.Label)
            .ThenBy(c => c.AccountName)
            .ToListAsync();
    }

    public async Task<PrestataireCredential?> GetByIdAsync(Guid id)
    {
        return await _context.PrestataireCredentials.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<PrestataireCredential> AddAsync(PrestataireCredential credential)
    {
        _context.PrestataireCredentials.Add(credential);
        await _context.SaveChangesAsync();
        return credential;
    }

    public async Task UpdateAsync(PrestataireCredential credential)
    {
        _context.Entry(credential).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var c = await _context.PrestataireCredentials.FirstOrDefaultAsync(c => c.Id == id);
        if (c != null)
        {
            _context.PrestataireCredentials.Remove(c);
            await _context.SaveChangesAsync();
        }
    }
}
