using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.TypesContrat.Queries.GetTypeContratById;

public class GetTypeContratByIdQueryHandler : IRequestHandler<GetTypeContratByIdQuery, TypeContrat?>
{
    private readonly ITypeContratRepository _repository;

    public GetTypeContratByIdQueryHandler(ITypeContratRepository repository)
    {
        _repository = repository;
    }

    public async Task<TypeContrat?> Handle(GetTypeContratByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
