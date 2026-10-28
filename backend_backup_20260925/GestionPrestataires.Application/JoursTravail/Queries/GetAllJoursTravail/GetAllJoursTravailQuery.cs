using MediatR;

namespace GestionPrestataires.Application.JoursTravail.Queries.GetAllJoursTravail;

/// <summary>
/// Query pour récupérer tous les jours travail
/// Avec filtrage selon le rôle de l'utilisateur
/// </summary>
public record GetAllJoursTravailQuery(
    Guid? PrestataireId = null,
    string? Mois = null,               //   Mois au lieu de Annee (format: "2025-12")
    string? UserRole = null,           //   AJOUT : Rôle de l'utilisateur
    string? UserPrestataireId = null   //   AJOUT : PrestataireId si User
) : IRequest<IEnumerable<object>>;  //   object pour retourner JourTravailDto OU JourTravailDtoRestricted