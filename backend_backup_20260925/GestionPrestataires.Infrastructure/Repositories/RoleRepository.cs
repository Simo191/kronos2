using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionRoles.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }
    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<Role> AddAsync(Role Role)
    {
        _context.Roles.Add(Role);
        await _context.SaveChangesAsync();
        return Role;
    }

    public async Task UpdateAsync(Role Role)
    {
        _context.Entry(Role).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var Role = await _context.Roles.FindAsync(id);
        if (Role != null)
        {
            _context.Roles.Remove(Role);
            await _context.SaveChangesAsync();
        }
    }
}
