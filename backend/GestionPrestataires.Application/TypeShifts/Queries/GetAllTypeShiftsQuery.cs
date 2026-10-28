using MediatR;

namespace GestionPrestataires.Application.TypeShifts.Queries
{
    public class GetAllTypeShiftsQuery : IRequest<List<TypeShift>>
    {
        public bool ActiveOnly { get; set; } = true;
    }
}
