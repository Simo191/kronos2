using MediatR;

namespace GestionPrestataires.Application.JoursTravail.Commands.UpdateJourTravail;

public record UpdateJourTravailCommand(
    Guid Id,
    Guid PrestataireId,
    string Mois, // Format: "2025-12"
    decimal NombreJours,
    decimal Tjm,
    string? Description
) : IRequest;
