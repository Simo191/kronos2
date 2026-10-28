using MediatR;
using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Application.Cagnottes.Queries.GetCagnotteActuelle;

public record GetCagnotteActuelleQuery : IRequest<Cagnotte?>;
