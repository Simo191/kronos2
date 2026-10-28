using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Prestataires.Queries.GetAllPrestataires;

public record GetAllPrestatairesQuery : IRequest<IEnumerable<Prestataire>>;
