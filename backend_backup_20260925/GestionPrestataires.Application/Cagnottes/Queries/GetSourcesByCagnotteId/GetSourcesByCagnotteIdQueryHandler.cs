using MediatR;
using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;

namespace GestionPrestataires.Application.Cagnottes.Queries.GetSourcesByCagnotteId;

public class GetSourcesByCagnotteIdQueryHandler : IRequestHandler<GetSourcesByCagnotteIdQuery, List<SourceBudget>>
{
    private readonly ISourceBudgetRepository _repository;

    public GetSourcesByCagnotteIdQueryHandler(ISourceBudgetRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<SourceBudget>> Handle(GetSourcesByCagnotteIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByCagnotteIdAsync(request.CagnotteId);
    }
}