using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Prestataires.Commands.DeletePrestataire;

public class DeletePrestataireCommandHandler : IRequestHandler<DeletePrestataireCommand>
{
    private readonly IPrestataireRepository _repository;

    public DeletePrestataireCommandHandler(IPrestataireRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeletePrestataireCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
    }
}
