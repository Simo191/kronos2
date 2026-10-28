using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Contrats.Commands.CreateContrat;

public class CreateContratCommandHandler : IRequestHandler<CreateContratCommand, Guid>
{
    private readonly IContratRepository _repository;

    public CreateContratCommandHandler(IContratRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateContratCommand request, CancellationToken cancellationToken)
    {
        var contrat = new Contrat
        {
            Id = Guid.NewGuid(),
            PrestataireId = request.PrestataireId,
            TypeContratId = request.TypeContratId,
            DateDebut = request.DateDebut,
            DateFin = request.DateFin,
            NombreJours = request.NombreJours,
            Tjm = request.Tjm,
            MontantTotal = request.MontantTotal,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            SeuilJoursMois = request.SeuilJoursMois,
            Active = request.Active,
            NumeroCommande = Nettoyer(request.NumeroCommande),
            ReferenceCtr = Nettoyer(request.ReferenceCtr),
            Tranche = TranchesContrat.Normaliser(request.Tranche),
            ApplicationId = request.ApplicationId,
            Commentaire = Nettoyer(request.Commentaire)
        };

        await _repository.AddAsync(contrat);

        if (request.Repartitions is { Count: > 0 })
            await _repository.ReplaceRepartitionsAsync(contrat.Id, RepartitionAnnuelleInput.VersEntites(request.Repartitions));

        return contrat.Id;
    }

    private static string? Nettoyer(string? valeur) =>
        string.IsNullOrWhiteSpace(valeur) ? null : valeur.Trim();
}
