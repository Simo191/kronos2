using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Cagnottes.Queries.GetCagnotteActuelle;

public record GetCagnotteActuelleQuery : IRequest<Cagnotte?>;
