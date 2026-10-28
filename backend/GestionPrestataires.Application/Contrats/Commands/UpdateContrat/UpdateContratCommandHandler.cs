using GestionPrestataires.Domain.Entities;
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
        contrat.NumeroCommande = Nettoyer(request.NumeroCommande);
        contrat.ReferenceCtr = Nettoyer(request.ReferenceCtr);
        if (request.Tranche != null)
            contrat.Tranche = TranchesContrat.Normaliser(request.Tranche);
        contrat.ApplicationId = request.ApplicationId;
        contrat.Commentaire = Nettoyer(request.Commentaire);

        await _repository.UpdateAsync(contrat);

        if (request.Repartitions != null)
            await _repository.ReplaceRepartitionsAsync(contrat.Id, RepartitionAnnuelleInput.VersEntites(request.Repartitions));
    }

    private static string? Nettoyer(string? valeur) =>
        string.IsNullOrWhiteSpace(valeur) ? null : valeur.Trim();
}
