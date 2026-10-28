using MediatR;

namespace GestionPrestataires.Application.MoisVerrouilles.Commands.VerrouillerMois;

public record VerrouillerMoisCommand(
    string Mois, // Format: "2025-12"
    Guid VerrouilleParUserId,
    string? Commentaire
) : IRequest<Guid>;