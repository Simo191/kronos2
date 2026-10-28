using MediatR;

namespace GestionPrestataires.Application.Contrats.Commands.UpdateContrat;

public record UpdateContratCommand(
    Guid Id,
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
    //   null = répartition inchangée ; liste (même vide) = remplace la répartition
    List<RepartitionAnnuelleInput>? Repartitions = null
) : IRequest;
