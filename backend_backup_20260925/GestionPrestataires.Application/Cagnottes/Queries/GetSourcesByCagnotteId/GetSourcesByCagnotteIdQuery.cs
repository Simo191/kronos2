using MediatR;
using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Application.Cagnottes.Queries.GetSourcesByCagnotteId;

public record GetSourcesByCagnotteIdQuery(Guid CagnotteId) : IRequest<List<SourceBudget>>;