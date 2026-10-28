using MediatR;

namespace GestionPrestataires.Application.JoursTravail.Commands.CreateJourTravail;

public record CreateJourTravailCommand(
    Guid PrestataireId,
    string Mois, // Format: "2025-12"
    decimal Tjm,
    decimal NombreJours,
    string? Description
) : IRequest<Guid>;
