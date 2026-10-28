using GestionPrestataires.Application.DTOs;
using MediatR;

namespace GestionPrestataires.Application.MoisVerrouilles.Queries.GetAllMoisVerrouilles;

public record GetAllMoisVerrouillesQuery : IRequest<IEnumerable<MoisVerrouillDto>>;