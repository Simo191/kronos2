using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Repositories
{

    public class ApplicationPrestataireAssignmentRepository : IApplicationPrestataireAssignmentRepository
    {
        private readonly AppDbContext _context;

        public ApplicationPrestataireAssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApplicationPrestataireAssignment>> GetAllAsync()
        {
            return await _context.ApplicationsPrestataireAssignments
                .ToListAsync();
        }
        public async Task<IEnumerable<ApplicationPrestataireAssignment>> GetIdApplicationAsync(Guid ApplicationId)
        {
            return await _context.ApplicationsPrestataireAssignments.Where(a => a.ApplicationId == ApplicationId)
                .ToListAsync();
        }
        public async Task<ApplicationPrestataireAssignment?> GetByIdAsync(Guid id)
        {
            return await _context.ApplicationsPrestataireAssignments

                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<ApplicationPrestataireAssignment> AddAsync(ApplicationPrestataireAssignment app)
        {
            _context.ApplicationsPrestataireAssignments.Add(app);
            await _context.SaveChangesAsync();
            return app;
        }
        public virtual async Task AddRangeAsync(IEnumerable<ApplicationPrestataireAssignment> app)
        {
            await _context.ApplicationsPrestataireAssignments.AddRangeAsync(app);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateAsync(ApplicationPrestataireAssignment app)
        {
            _context.ApplicationsPrestataireAssignments.Update(app);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var app = await _context.ApplicationsPrestataireAssignments
                .FirstOrDefaultAsync(a => a.Id == id);
            if (app != null)
            {
                _context.ApplicationsPrestataireAssignments.Remove(app);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteAllAsync(Guid ApplicationId)
        {
            var app = await GetIdApplicationAsync(ApplicationId);
            _context.ApplicationsPrestataireAssignments.RemoveRange(app);
            await _context.SaveChangesAsync();

        }
    }
}