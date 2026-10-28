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
    public class ShiftRepository : Repository<Shift>, IShiftRepository
    {
        public ShiftRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Shift>> GetShiftsByPrestataireAsync(Guid prestataireId)
        {
            return await _dbSet
                .Include(s => s.TypeShift)
                .Where(s => s.PrestataireId == prestataireId)
                .OrderBy(s => s.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shift>> GetShiftsByMonthAsync(int year, int month, Guid? prestataireId = null)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var query = _dbSet
                .Include(s => s.TypeShift)
                .Where(s => s.Date >= startDate && s.Date <= endDate);

            if (prestataireId.HasValue)
            {
                query = query.Where(s => s.PrestataireId == prestataireId.Value);
            }

            return await query
                .OrderBy(s => s.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shift>> GetShiftsByWeekAsync(DateTime weekStartDate, Guid? prestataireId = null)
        {
            var endDate = weekStartDate.AddDays(6);

            var query = _dbSet
                .Include(s => s.TypeShift)
                .Where(s => s.Date >= weekStartDate && s.Date <= endDate);

            if (prestataireId.HasValue)
            {
                query = query.Where(s => s.PrestataireId == prestataireId.Value);
            }

            return await query
                .OrderBy(s => s.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shift>> GetShiftsByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? prestataireId = null)
        {
            var query = _dbSet.Include(s => s.Prestataire)
                .Include(s => s.TypeShift)
                .Where(s => s.Date >= startDate && s.Date <= endDate);

            if (prestataireId.HasValue)
            {
                query = query.Where(s => s.PrestataireId == prestataireId.Value);
            }

            return await query
                .OrderBy(s => s.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shift>> GetShiftsByTypeAsync(Guid typeShiftId)
        {
            return await _dbSet
                .Include(s => s.TypeShift)
                .Where(s => s.TypeShiftId == typeShiftId)
                .OrderBy(s => s.Date)
                .ToListAsync();
        }

        public async Task<bool> HasShiftOnDateAsync(Guid prestataireId, DateTime date)
        {
            return await _dbSet
                .AnyAsync(s => s.PrestataireId == prestataireId && s.Date.Date == date.Date);
        }
    }
}
