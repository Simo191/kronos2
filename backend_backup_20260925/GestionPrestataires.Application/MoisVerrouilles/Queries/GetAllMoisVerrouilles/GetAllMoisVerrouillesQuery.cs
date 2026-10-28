using MediatR;
using GestionPrestataires.Application.DTOs;

namespace GestionPrestataires.Application.MoisVerrouilles.Queries.GetAllMoisVerrouilles;

public record GetAllMoisVerrouillesQuery : IRequest<IEnumerable<MoisVerrouillDto>>;