using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Prestataires.Queries.GetAllPrestataires;

//   IncludeInactive=true uniquement pour la page Liste Prestataires (admin/manager).
//   Par défaut false : les modules consommateurs (shifts, contrats, sélecteurs, etc.)
//   ne voient JAMAIS les prestataires désactivés.
public record GetAllPrestatairesQuery(bool IncludeInactive = false) : IRequest<IEnumerable<Prestataire>>;
