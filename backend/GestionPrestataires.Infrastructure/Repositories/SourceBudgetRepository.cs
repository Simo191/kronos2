using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Repositories;

public class SourceBudgetRepository : ISourceBudgetRepository
{
    private readonly AppDbContext _context;

    public SourceBudgetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SourceBudget?> GetByIdAsync(Guid id)
    {
        return await _context.SourceBudgets
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<SourceBudget>> GetByCagnotteIdAsync(Guid cagnotteId)
    {
        return await _context.SourceBudgets
            .Where(s => s.CagnotteId == cagnotteId)
            .OrderBy(s => s.Code)
            .ToListAsync();
    }

    public async Task<SourceBudget> AddAsync(SourceBudget sourceBudget)
    {
        _context.SourceBudgets.Add(sourceBudget);
        await _context.SaveChangesAsync();
        return sourceBudget;
    }

    public async Task UpdateAsync(SourceBudget sourceBudget)
    {
        sourceBudget.DateModification = DateTime.UtcNow;
        _context.SourceBudgets.Update(sourceBudget);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var source = await GetByIdAsync(id);
        if (source != null)
        {
            _context.SourceBudgets.Remove(source);
            await _context.SaveChangesAsync();
        }
    }
}