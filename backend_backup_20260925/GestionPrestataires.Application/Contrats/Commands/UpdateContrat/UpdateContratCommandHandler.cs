using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Contrats.Commands.UpdateContrat;

public class UpdateContratCommandHandler : IRequestHandler<UpdateContratCommand>
{
    private readonly IContratRepository _repository;

    public UpdateContratCommandHandler(IContratRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateContratCommand request, CancellationToken cancellationToken)
    {
        var contrat = await _repository.GetByIdAsync(request.Id);
        if (contrat == null)
            throw new Exception("Contrat not found");

        contrat.PrestataireId = request.PrestataireId;
        contrat.TypeContratId = request.TypeContratId;
        contrat.DateDebut = request.DateDebut;
        contrat.DateFin = request.DateFin;
        contrat.NombreJours = request.NombreJours;
        contrat.Tjm = request.Tjm;
        contrat.MontantTotal = request.MontantTotal;
        contrat.UpdatedAt = DateTime.UtcNow;
        contrat.SeuilJoursMois = request.SeuilJoursMois;
        contrat.Active = request.Active;
 
        await _repository.UpdateAsync(contrat);
    }
}
