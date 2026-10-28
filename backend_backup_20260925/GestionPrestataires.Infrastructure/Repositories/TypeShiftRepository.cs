using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Infrastructure.Repositories
{
    public class TypeShiftRepository : Repository<TypeShift>, ITypeShiftRepository
    {
        public TypeShiftRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TypeShift>> GetActiveTypesAsync()
        {
            return await _dbSet
                .Where(t => t.IsActive)
                .OrderBy(t => t.Order)
                .ToListAsync();
        }
        public async Task<IEnumerable<TypeShift>> GetALLSypesAsync()
        {
            return await _dbSet
                .Where(t => t.IsActive)
                .OrderBy(t => t.Order)
                .ToListAsync();
        }
        public async Task<TypeShift> GetByCodeAsync(string code)
        {
            return await _dbSet
                .FirstOrDefaultAsync(t => t.Code == code);
        }

        public async Task<IEnumerable<TypeShift>> GetOrderedTypesAsync()
        {
            return await _dbSet
                .OrderBy(t => t.Order)
                .ToListAsync();
        }

        public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null)
        {
            var query = _dbSet.Where(t => t.Code == code);

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
