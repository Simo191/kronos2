using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.TypeShifts.Queries;

public class GetTypeShiftByIdQueryHandler : IRequestHandler<GetTypeShiftByIdQuery, TypeShift?>
{
    private readonly ITypeShiftRepository _repository;

    public GetTypeShiftByIdQueryHandler(ITypeShiftRepository repository)
    {
        _repository = repository;
    }

    public async Task<TypeShift?> Handle(GetTypeShiftByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
