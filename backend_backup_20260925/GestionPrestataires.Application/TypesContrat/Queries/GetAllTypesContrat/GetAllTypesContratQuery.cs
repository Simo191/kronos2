using MediatR;
using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Application.TypesContrat.Queries.GetAllTypesContrat;

public record GetAllTypesContratQuery : IRequest<List<TypeContrat>>;
