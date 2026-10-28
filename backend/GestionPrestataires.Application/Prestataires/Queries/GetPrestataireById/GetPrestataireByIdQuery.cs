using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Prestataires.Queries.GetPrestataireById;

public record GetPrestataireByIdQuery(Guid Id) : IRequest<Prestataire?>;
