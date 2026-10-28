using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Repositories;

public class ApplicationServerRepository : IApplicationServerRepository
{
    private readonly AppDbContext _context;

    public ApplicationServerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task DeleteAllAsync(Guid applicationId)
    {
        //   ExecuteDeleteAsync = SQL DELETE direct, n'altère pas le change tracker
        //   (évite les conflits "row 0 affected" quand l'entité parent est déjà tracked).
        await _context.ApplicationServers
            .Where(s => s.ApplicationId == applicationId)
            .ExecuteDeleteAsync();
    }

    public async Task AddRangeAsync(IEnumerable<ApplicationServer> servers)
    {
        var list = servers?.ToList() ?? new List<ApplicationServer>();
        if (list.Count == 0) return;

        await _context.ApplicationServers.AddRangeAsync(list);
        await _context.SaveChangesAsync();
    }
}
