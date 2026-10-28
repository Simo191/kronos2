using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Domain.Repositories
{
    public interface IShiftRepository : IRepository<Shift>
    {
        Task<IEnumerable<Shift>> GetShiftsByPrestataireAsync(Guid prestataireId);
        Task<IEnumerable<Shift>> GetShiftsByMonthAsync(int year, int month, Guid? prestataireId = null);
        Task<IEnumerable<Shift>> GetShiftsByWeekAsync(DateTime weekStartDate, Guid? prestataireId = null);
        Task<IEnumerable<Shift>> GetShiftsByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? prestataireId = null);
        Task<IEnumerable<Shift>> GetShiftsByTypeAsync(Guid typeShiftId);
        Task<bool> HasShiftOnDateAsync(Guid prestataireId, DateTime date);
    }
}
