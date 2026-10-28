using GestionPrestataires.Application.DTOs;
using MediatR;

namespace GestionPrestataires.Application.Estimations.Queries.GetAllEstimations;

/// <summary>
/// Query pour récupérer toutes les estimations
/// </summary>
public class GetAllEstimationsQuery : IRequest<List<EstimationDto>>
{
}