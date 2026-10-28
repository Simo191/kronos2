using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Prestataires.Queries.GetPrestataireById;

public class GetPrestataireByIdQueryHandler : IRequestHandler<GetPrestataireByIdQuery, Prestataire?>
{
    private readonly IPrestataireRepository _repository;

    public GetPrestataireByIdQueryHandler(IPrestataireRepository repository)
    {
        _repository = repository;
    }

    public async Task<Prestataire?> Handle(GetPrestataireByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
