using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Contrats.Commands.UpdateContratApplication;

public class UpdateContratApplicationCommandHandler : IRequestHandler<UpdateContratApplicationCommand, bool>
{
    private readonly IContratRepository _repository;

    public UpdateContratApplicationCommandHandler(IContratRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateContratApplicationCommand request, CancellationToken cancellationToken)
    {
        var contrat = await _repository.GetByIdAsync(request.Id);
        if (contrat == null) return false;

        contrat.ApplicationId = request.ApplicationId;
        contrat.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(contrat);
        return true;
    }
}
