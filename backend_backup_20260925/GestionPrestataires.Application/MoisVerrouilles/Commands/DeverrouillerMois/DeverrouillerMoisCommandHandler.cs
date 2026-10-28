using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.MoisVerrouilles.Commands.DeverrouillerMois;

public class DeverrouillerMoisCommandHandler : IRequestHandler<DeverrouillerMoisCommand>
{
    private readonly IMoisVerrouillRepository _repository;

    public DeverrouillerMoisCommandHandler(IMoisVerrouillRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeverrouillerMoisCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
    }
}