using MediatR;

namespace GestionPrestataires.Application.Contrats.Commands.CreateContrat;

public record CreateContratCommand(
    Guid PrestataireId,
    Guid TypeContratId,
    DateTime DateDebut,
    DateTime DateFin,
    decimal NombreJours,
    decimal Tjm,
    decimal MontantTotal,
     decimal? SeuilJoursMois,
    bool Active,
    //   Suivi des commandes (facultatifs)
    string? NumeroCommande = null,
    string? ReferenceCtr = null,
    string? Tranche = null,
    Guid? ApplicationId = null,
    string? Commentaire = null,
    List<RepartitionAnnuelleInput>? Repartitions = null
) : IRequest<Guid>;
