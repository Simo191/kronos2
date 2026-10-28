using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Cagnottes.Queries.GetAllCagnottes;

public record GetAllCagnottesQuery : IRequest<List<Cagnotte>>;
