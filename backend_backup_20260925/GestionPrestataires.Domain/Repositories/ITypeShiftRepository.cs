using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Domain.Repositories
{
    public interface ITypeShiftRepository : IRepository<TypeShift>
    {
        Task<IEnumerable<TypeShift>> GetActiveTypesAsync();
        Task<TypeShift> GetByCodeAsync(string code);
        Task<IEnumerable<TypeShift>> GetOrderedTypesAsync();
        Task<bool> CodeExistsAsync(string code, Guid? excludeId = null);
        Task<IEnumerable<TypeShift>> GetALLSypesAsync();
    }
}
