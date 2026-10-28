using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Persistence.Repositories;

public class ApplicationPlanningRepository : IApplicationPlanningRepository
{
    private readonly AppDbContext _context;

    public ApplicationPlanningRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApplicationPlanning>> GetAllAsync()
    {
        return await _context.ApplicationsPlannings
            .Include(a => a.Assignments)
                .ThenInclude(ass => ass.Prestataire)
                    .ThenInclude(p => p!.Entreprise)
            .Include(a => a.Servers)
            .OrderBy(a => a.Ordre)
            .ThenBy(a => a.DateDebut)
            .ToListAsync();
    }

    public async Task<ApplicationPlanning?> GetByIdAsync(Guid id)
    {
        return await _context.ApplicationsPlannings
            .Include(a => a.Assignments)
                .ThenInclude(ass => ass.Prestataire)
                    .ThenInclude(p => p!.Entreprise)
            .Include(a => a.Servers)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<ApplicationPlanning> AddAsync(ApplicationPlanning app)
    {
        _context.ApplicationsPlannings.Add(app);
        await _context.SaveChangesAsync();
        return app;
    }

    public async Task UpdateAsync(ApplicationPlanning app)
    {
        //   On NE fait PAS `_context.Update(app)` ici : ça re-tracke tout le graph
        //   (Servers, Assignments…) en Modified — ce qui plante quand on a déjà
        //   supprimé les enfants en DB via repos dédiés (ExecuteDeleteAsync).
        //
        //   L'entité root est déjà tracked depuis GetByIdAsync ; SaveChanges
        //   détecte automatiquement les modifs scalaires (Nom, Dates, Statut…).
        //   On force EntityState.Modified sur la root uniquement pour garantir
        //   l'UPDATE même si aucune propriété n'a réellement changé.
        _context.Entry(app).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var app = await _context.ApplicationsPlannings
            .Include(a => a.Assignments)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (app != null)
        {
            _context.ApplicationsPlannings.Remove(app);
            await _context.SaveChangesAsync();
        }
    }
}