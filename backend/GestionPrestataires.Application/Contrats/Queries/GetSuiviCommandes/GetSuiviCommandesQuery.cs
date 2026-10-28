using MediatR;

namespace GestionPrestataires.Application.Contrats.Queries.GetSuiviCommandes;

public record GetSuiviCommandesQuery : IRequest<SuiviCommandesDto>;
