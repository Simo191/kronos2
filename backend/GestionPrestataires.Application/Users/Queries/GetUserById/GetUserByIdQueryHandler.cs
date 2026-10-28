using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User?>
{
    private readonly IUserRepository _repository;

    public GetUserByIdQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<User?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
