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
    bool Active
) : IRequest;
