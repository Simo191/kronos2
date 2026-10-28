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
    bool Active
) : IRequest<Guid>;
