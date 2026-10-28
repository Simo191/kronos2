using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.TypesContrat.Commands.UpdateTypeContrat;

public class UpdateTypeContratCommandHandler : IRequestHandler<UpdateTypeContratCommand, Unit>
{
    private readonly ITypeContratRepository _repository;

    public UpdateTypeContratCommandHandler(ITypeContratRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateTypeContratCommand request, CancellationToken cancellationToken)
    {
        var typeContrat = await _repository.GetByIdAsync(request.Id);
        if (typeContrat == null)
            throw new Exception("Type de contrat non trouvé");

        typeContrat.Libelle = request.Libelle;
        typeContrat.Description = request.Description!;

        await _repository.UpdateAsync(typeContrat);
        return Unit.Value;
    }
}
