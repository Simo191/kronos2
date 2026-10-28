using MediatR;
using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Application.Cagnottes.Queries.GetAllCagnottes;

public record GetAllCagnottesQuery : IRequest<List<Cagnotte>>;
