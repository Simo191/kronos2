using MediatR;
using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Application.TypesContrat.Queries.GetTypeContratById;

public record GetTypeContratByIdQuery(Guid Id) : IRequest<TypeContrat?>;
