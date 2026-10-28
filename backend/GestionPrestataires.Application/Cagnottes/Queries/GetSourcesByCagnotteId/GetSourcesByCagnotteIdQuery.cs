using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Cagnottes.Queries.GetSourcesByCagnotteId;

public record GetSourcesByCagnotteIdQuery(Guid CagnotteId) : IRequest<List<SourceBudget>>;