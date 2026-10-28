using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.TypesContrat.Queries.GetTypeContratById;

public record GetTypeContratByIdQuery(Guid Id) : IRequest<TypeContrat?>;
