using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Estimations.Commands.DeleteEstimation;

/// <summary>
/// Handler pour DeleteEstimationCommand
/// </summary>
public class DeleteEstimationCommandHandler : IRequestHandler<DeleteEstimationCommand, Unit>
{
    private readonly IEstimationRepository _repository;

    public DeleteEstimationCommandHandler(IEstimationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteEstimationCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
        return Unit.Value;
    }
}