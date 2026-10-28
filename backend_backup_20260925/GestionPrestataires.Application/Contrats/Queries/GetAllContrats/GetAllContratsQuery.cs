using MediatR;
using GestionPrestataires.Application.DTOs;

namespace GestionPrestataires.Application.Contrats.Queries.GetAllContrats;

/// <summary>
/// Query pour récupérer tous les contrats
/// Avec filtrage selon le rôle de l'utilisateur
/// </summary>
public record GetAllContratsQuery(
    string? UserRole = null,           //   AJOUT : Rôle de l'utilisateur
    string? UserPrestataireId = null   //   AJOUT : PrestataireId si User
) : IRequest<IEnumerable<object>>;  //   object pour pouvoir retourner ContratDto OU ContratDtoRestricted