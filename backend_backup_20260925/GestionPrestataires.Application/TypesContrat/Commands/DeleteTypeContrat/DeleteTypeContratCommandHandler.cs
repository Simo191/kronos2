using MediatR;
using GestionPrestataires.Domain.Repositories;

namespace GestionPrestataires.Application.TypesContrat.Commands.DeleteTypeContrat;

public class DeleteTypeContratCommandHandler : IRequestHandler<DeleteTypeContratCommand, Unit>
{
    private readonly ITypeContratRepository _repository;

    public DeleteTypeContratCommandHandler(ITypeContratRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteTypeContratCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
        return Unit.Value;
    }
}
