using GestionPrestataires.Application.DTOs;
using MediatR;

namespace GestionPrestataires.Application.Contrats.Queries.GetContratById;

//   MODIFIÉ : Retourne ContratDto au lieu de Contrat
public record GetContratByIdQuery(Guid Id) : IRequest<ContratDto?>;