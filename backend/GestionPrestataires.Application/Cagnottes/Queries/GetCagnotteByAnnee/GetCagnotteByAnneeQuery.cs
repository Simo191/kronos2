using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Cagnottes.Queries.GetCagnotteByAnnee;

public record GetCagnotteByAnneeQuery(int Annee) : IRequest<Cagnotte?>;
