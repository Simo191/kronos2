using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<IEnumerable<User>>;
