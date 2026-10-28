using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<User>>
{
    private readonly IUserRepository _repository;

    public GetAllUsersQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}
