using GestionPrestataires.Domain.Entities;
using MediatR;

namespace GestionPrestataires.Application.Users.Queries.GetUserByEmail;
public record GetUserByEmailQuery(string Email) : IRequest<User?>;
