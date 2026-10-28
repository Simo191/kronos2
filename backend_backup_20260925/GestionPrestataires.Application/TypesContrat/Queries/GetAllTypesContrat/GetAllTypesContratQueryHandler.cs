using MediatR;
using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using System.Linq; // important

namespace GestionPrestataires.Application.TypesContrat.Queries.GetAllTypesContrat;

public class GetAllTypesContratQueryHandler
    : IRequestHandler<GetAllTypesContratQuery, List<TypeContrat>>
{
    private readonly ITypeContratRepository _repository;

    public GetAllTypesContratQueryHandler(ITypeContratRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TypeContrat>> Handle(
        GetAllTypesContratQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync();
        return items.ToList();
    }
}
