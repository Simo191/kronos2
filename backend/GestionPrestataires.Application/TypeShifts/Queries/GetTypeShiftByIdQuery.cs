using MediatR;

namespace GestionPrestataires.Application.TypeShifts.Queries
{

    public class GetTypeShiftByIdQuery : IRequest<TypeShift>
    {
        public Guid Id { get; set; }
    }
}
