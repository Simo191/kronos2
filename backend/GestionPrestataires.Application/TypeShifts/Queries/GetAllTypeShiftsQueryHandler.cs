using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.TypeShifts.Queries
{
    public class GetAllTypeShiftsQueryHandler : IRequestHandler<GetAllTypeShiftsQuery, List<TypeShift>>
    {
        private readonly ITypeShiftRepository _context;

        public GetAllTypeShiftsQueryHandler(ITypeShiftRepository context)
        {
            _context = context;
        }

        public async Task<List<TypeShift>> Handle(GetAllTypeShiftsQuery request, CancellationToken cancellationToken)
        {
            var query = await _context.GetALLSypesAsync();

            return query.ToList(); ;
        }
    }
}
