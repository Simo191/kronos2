using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Prestataires.Queries.GetAllPrestataires;

public class GetAllPrestatairesQueryHandler : IRequestHandler<GetAllPrestatairesQuery, IEnumerable<Prestataire>>
{
    private readonly IPrestataireRepository _repository;

    public GetAllPrestatairesQueryHandler(IPrestataireRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Prestataire>> Handle(GetAllPrestatairesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}
