using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Contrats.Commands.UpdateContratDocument;

public class UpdateContratDocumentCommandHandler : IRequestHandler<UpdateContratDocumentCommand>
{
    private readonly IContratRepository _repository;

    public UpdateContratDocumentCommandHandler(IContratRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateContratDocumentCommand request, CancellationToken cancellationToken)
    {
        var contrat = await _repository.GetByIdAsync(request.Id);
        if (contrat == null)
            throw new Exception("Contrat introuvable");

        contrat.DocumentUrl = request.DocumentUrl;
        contrat.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(contrat);
    }
}