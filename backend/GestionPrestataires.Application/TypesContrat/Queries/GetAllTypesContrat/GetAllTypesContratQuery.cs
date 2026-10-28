using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.TypesContrat.Queries.GetAllTypesContrat;

public record GetAllTypesContratQuery : IRequest<List<TypeContrat>>;
