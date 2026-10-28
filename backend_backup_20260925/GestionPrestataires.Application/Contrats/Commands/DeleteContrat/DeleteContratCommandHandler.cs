using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Contrats.Commands.DeleteContrat;

public class DeleteContratCommandHandler : IRequestHandler<DeleteContratCommand>
{
    private readonly IContratRepository _repository;

    public DeleteContratCommandHandler(IContratRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteContratCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
    }
}
