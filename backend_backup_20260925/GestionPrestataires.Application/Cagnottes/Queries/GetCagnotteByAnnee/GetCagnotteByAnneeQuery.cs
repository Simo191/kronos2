using MediatR;
using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Application.Cagnottes.Queries.GetCagnotteByAnnee;

public record GetCagnotteByAnneeQuery(int Annee) : IRequest<Cagnotte?>;
