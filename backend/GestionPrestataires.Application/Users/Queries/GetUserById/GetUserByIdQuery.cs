using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<User?>;
