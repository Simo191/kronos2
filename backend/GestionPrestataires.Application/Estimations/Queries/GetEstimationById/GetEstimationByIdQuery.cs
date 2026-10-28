using GestionPrestataires.Application.DTOs;
using MediatR;

namespace GestionPrestataires.Application.Estimations.Queries.GetEstimationById;
/// <summary>
/// Query pour récupérer une estimation par ID
/// </summary>
public class GetEstimationByIdQuery : IRequest<EstimationDto?>
{
    public Guid Id { get; set; }

    public GetEstimationByIdQuery(Guid id)
    {
        Id = id;
    }
}
