using GestionPrestataires.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
