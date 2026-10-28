using MediatR;

namespace GestionPrestataires.Application.Prestataires.Commands.ToggleActivePrestataire;

//   Bascule l'activation d'un prestataire.
//   Si un User est lié, son IsActive est synchronisé (bloque la connexion).
//   - Reason : motif facultatif (utilisé uniquement lors d'une désactivation).
//   - Retourne le nouvel état (true = actif, false = désactivé).
public record ToggleActivePrestataireCommand(Guid Id, string? Reason = null) : IRequest<bool>;
