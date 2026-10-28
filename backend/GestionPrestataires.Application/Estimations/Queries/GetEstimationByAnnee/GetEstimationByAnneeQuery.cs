using GestionPrestataires.Application.DTOs;
using MediatR;

namespace GestionPrestataires.Application.Estimations.Queries.GetEstimationByAnnee;
/// <summary>
/// Query pour récupérer une estimation par année
/// </summary>
public class GetEstimationByAnneeQuery : IRequest<EstimationDto?>
{
    public int Annee { get; set; }

    public GetEstimationByAnneeQuery(int annee)
    {
        Annee = annee;
    }
}